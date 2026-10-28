namespace Conde.Golomb;

/// <summary>
/// O juiz. Não sabe nada de busca, de poda, de cota: recebe uma régua pronta e
/// responde pela DEFINIÇÃO, olhando a lista de distâncias que a régua produz.
/// Tudo que a busca afirma passa por aqui antes de virar número de README.
/// </summary>
public static class Juiz
{
    /// <summary>
    /// É régua de Golomb quando nenhuma distância aparece duas vezes. Dito de
    /// outro jeito: a régua nunca mede o mesmo valor por dois pares de marcas.
    /// </summary>
    public static bool EhGolomb(Regua regua)
    {
        ArgumentNullException.ThrowIfNull(regua);
        var distancias = regua.Distancias();
        for (var i = 1; i < distancias.Length; i++)
            if (distancias[i] == distancias[i - 1])
                return false;
        return true;
    }

    /// <summary>
    /// O primeiro par de marcas que mede a mesma distância de outro par, ou null
    /// se não houver. Serve para a mensagem de erro dizer QUAL é o defeito, em vez
    /// de só dizer que existe.
    /// </summary>
    public static (int a, int b, int c, int d, int distancia)? PrimeiraColisao(Regua regua)
    {
        ArgumentNullException.ThrowIfNull(regua);
        var vistos = new Dictionary<int, (int, int)>();
        var marcas = regua.Marcas;
        for (var i = 0; i < marcas.Count; i++)
            for (var j = i + 1; j < marcas.Count; j++)
            {
                var d = marcas[j] - marcas[i];
                if (vistos.TryGetValue(d, out var antes))
                    return (antes.Item1, antes.Item2, marcas[i], marcas[j], d);
                vistos[d] = (marcas[i], marcas[j]);
            }
        return null;
    }

    /// <summary>
    /// Régua perfeita: é de Golomb E mede todas as distâncias de 1 ao comprimento,
    /// sem buraco. Com n marcas são n(n-1)/2 distâncias distintas, então só pode
    /// ser perfeita quando o comprimento é exatamente n(n-1)/2.
    /// </summary>
    public static bool EhPerfeita(Regua regua)
    {
        ArgumentNullException.ThrowIfNull(regua);
        if (!EhGolomb(regua)) return false;
        var distancias = regua.Distancias();
        if (regua.Comprimento != distancias.Length) return false;
        for (var i = 0; i < distancias.Length; i++)
            if (distancias[i] != i + 1)
                return false;
        return true;
    }

    /// <summary>
    /// Quantas distâncias de 1 a <paramref name="ate"/> a régua consegue medir.
    /// É o que separa uma régua boa de uma régua curta: medir muito não é o mesmo
    /// que ser pequena.
    /// </summary>
    public static int QuantasMede(Regua regua, int ate)
    {
        ArgumentNullException.ThrowIfNull(regua);
        var tem = new bool[ate + 1];
        foreach (var d in regua.Distancias())
            if (d <= ate)
                tem[d] = true;
        var total = 0;
        for (var d = 1; d <= ate; d++)
            if (tem[d])
                total++;
        return total;
    }

    /// <summary>
    /// Confere a afirmação "esta é a menor régua com n marcas" do único jeito que
    /// não depende da busca: toda régua de Golomb com n marcas e comprimento menor
    /// teria de não existir, e aqui isso é verificado marca a marca, sem poda
    /// nenhuma, no braço. Só serve para n pequeno, e é exatamente para isso que
    /// existe: ser lento e burro o bastante para não compartilhar erro com a busca.
    /// </summary>
    public static bool NaoExisteMenor(int ordem, int comprimento)
    {
        if (ordem < 1) throw new ArgumentOutOfRangeException(nameof(ordem));
        for (var limite = ordem - 1; limite < comprimento; limite++)
            if (ExisteComComprimento(ordem, limite))
                return false;
        return true;
    }

    private static bool ExisteComComprimento(int ordem, int comprimento)
    {
        if (ordem == 1) return comprimento == 0;
        var marcas = new int[ordem];
        marcas[0] = 0;
        marcas[ordem - 1] = comprimento;
        return Completa(marcas, 1, ordem, comprimento);
    }

    private static bool Completa(int[] marcas, int pos, int ordem, int comprimento)
    {
        if (pos == ordem - 1)
            return EhGolomb(new Regua((int[])marcas.Clone()));

        var inicio = marcas[pos - 1] + 1;
        for (var v = inicio; v < comprimento; v++)
        {
            marcas[pos] = v;
            if (Completa(marcas, pos + 1, ordem, comprimento)) return true;
        }
        return false;
    }
}
