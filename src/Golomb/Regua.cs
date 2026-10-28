namespace Conde.Golomb;

/// <summary>
/// Uma régua: marcas inteiras, em ordem, começando em zero. A primeira marca é
/// sempre 0 porque deslocar a régua inteira não muda distância nenhuma, e manter
/// essa normalização no tipo evita contar a mesma régua n vezes na varredura.
/// </summary>
public sealed class Regua : IEquatable<Regua>
{
    private readonly int[] _marcas;

    public Regua(params int[] marcas)
    {
        ArgumentNullException.ThrowIfNull(marcas);
        if (marcas.Length == 0) throw new ArgumentException("régua sem marca nenhuma", nameof(marcas));

        var copia = (int[])marcas.Clone();
        Array.Sort(copia);
        if (copia[0] != 0) throw new ArgumentException("a primeira marca tem de ser 0", nameof(marcas));
        for (var i = 1; i < copia.Length; i++)
            if (copia[i] == copia[i - 1])
                throw new ArgumentException("marca repetida: " + copia[i], nameof(marcas));

        _marcas = copia;
    }

    /// <summary>As marcas, em ordem crescente, começando em 0.</summary>
    public IReadOnlyList<int> Marcas => _marcas;

    /// <summary>Quantas marcas a régua tem. É a "ordem" da régua.</summary>
    public int Ordem => _marcas.Length;

    /// <summary>A última marca, que é o tamanho do pedaço de madeira.</summary>
    public int Comprimento => _marcas[^1];

    /// <summary>
    /// As n(n-1)/2 distâncias entre pares de marcas, com repetição, em ordem
    /// crescente. É a lista do que a régua consegue medir.
    /// </summary>
    public int[] Distancias()
    {
        var saida = new int[Ordem * (Ordem - 1) / 2];
        var k = 0;
        for (var i = 0; i < _marcas.Length; i++)
            for (var j = i + 1; j < _marcas.Length; j++)
                saida[k++] = _marcas[j] - _marcas[i];
        Array.Sort(saida);
        return saida;
    }

    /// <summary>A mesma régua virada do avesso. Mede exatamente as mesmas distâncias.</summary>
    public Regua Espelho()
    {
        var saida = new int[Ordem];
        for (var i = 0; i < Ordem; i++) saida[i] = Comprimento - _marcas[Ordem - 1 - i];
        return new Regua(saida);
    }

    /// <summary>
    /// A representante do par {régua, espelho}: a menor das duas na ordem
    /// lexicográfica. Duas réguas com a mesma canônica medem o mesmo.
    /// </summary>
    public Regua Canonica()
    {
        var espelho = Espelho();
        return Compara(this, espelho) <= 0 ? this : espelho;
    }

    public bool Equals(Regua? outra) => outra is not null && _marcas.AsSpan().SequenceEqual(outra._marcas);

    public override bool Equals(object? obj) => Equals(obj as Regua);

    public override int GetHashCode()
    {
        var h = new HashCode();
        foreach (var m in _marcas) h.Add(m);
        return h.ToHashCode();
    }

    public override string ToString() => string.Join(" ", _marcas);

    private static int Compara(Regua a, Regua b)
    {
        var n = Math.Min(a.Ordem, b.Ordem);
        for (var i = 0; i < n; i++)
        {
            var c = a._marcas[i].CompareTo(b._marcas[i]);
            if (c != 0) return c;
        }
        return a.Ordem.CompareTo(b.Ordem);
    }
}
