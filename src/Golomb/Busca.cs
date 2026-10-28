namespace Conde.Golomb;

/// <summary>O que uma varredura devolve: as réguas achadas e o que custou achá-las.</summary>
public readonly record struct Achado(int Ordem, int Comprimento, IReadOnlyList<Regua> Reguas, long Nos)
{
    public bool Existe => Reguas.Count > 0;
}

/// <summary>
/// A varredura. Monta réguas marca por marca, da esquerda para a direita, e
/// desiste assim que duas marcas medem a mesma distância.
/// </summary>
public static class Busca
{
    /// <summary>
    /// Todas as réguas de Golomb com <paramref name="ordem"/> marcas e exatamente
    /// o comprimento pedido.
    /// </summary>
    /// <param name="podaEspelho">
    /// Quando ligada, só aceita a régua cuja primeira distância é menor que a
    /// última, ficando com uma de cada par {régua, espelho}. Numa régua de Golomb
    /// as duas nunca são iguais, então a poda não perde nada: ela corta exatamente
    /// o espelho.
    /// </param>
    /// <param name="minimos">
    /// Comprimento mínimo conhecido de uma régua com k marcas, indexado por k.
    /// Serve de poda: se o que sobra do pedaço de madeira já é menor que o mínimo
    /// das marcas que faltam, não adianta continuar. Passar null desliga a poda.
    /// </param>
    public static Achado Reguas(int ordem, int comprimento, bool podaEspelho = true, IReadOnlyList<int>? minimos = null)
    {
        if (ordem < 1) throw new ArgumentOutOfRangeException(nameof(ordem));
        if (comprimento < 0) throw new ArgumentOutOfRangeException(nameof(comprimento));

        var achadas = new List<Regua>();
        long nos = 0;

        if (ordem == 1)
            return new Achado(1, comprimento, comprimento == 0 ? new List<Regua> { new(0) } : achadas, 0);

        // n marcas distintas não cabem num pedaço mais curto que n-1.
        if (comprimento < ordem - 1) return new Achado(ordem, comprimento, achadas, 0);

        var marcas = new int[ordem];
        var usada = new bool[comprimento + 1];
        marcas[0] = 0;

        void Passo(int pos)
        {
            if (pos == ordem - 1)
            {
                // A última marca é o comprimento: não há escolha, só conferência. E a
                // conferência é contra TODAS as marcas já postas, não só contra a
                // anterior: foi assim que a primeira versão desta busca jurou que a
                // menor régua de 5 marcas tinha comprimento 10.
                nos++;
                if (podaEspelho && marcas[1] > comprimento - marcas[pos - 1]) return;

                var postas = 0;
                var ok = true;
                for (var i = 0; i < pos; i++)
                {
                    var d = comprimento - marcas[i];
                    if (usada[d]) { ok = false; break; }
                    usada[d] = true;
                    postas++;
                }

                if (ok)
                {
                    marcas[pos] = comprimento;
                    achadas.Add(new Regua((int[])marcas.Clone()));
                }

                for (var i = 0; i < postas; i++) usada[comprimento - marcas[i]] = false;
                return;
            }

            var inicio = marcas[pos - 1] + 1;
            var faltam = ordem - pos;                       // marcas ainda por colocar, contando esta
            var reserva = minimos is not null && faltam < minimos.Count ? minimos[faltam] : faltam - 1;

            for (var v = inicio; v <= comprimento - reserva; v++)
            {
                nos++;
                var postas = 0;
                var ok = true;
                for (var i = 0; i < pos; i++)
                {
                    var d = v - marcas[i];
                    if (usada[d]) { ok = false; break; }
                    usada[d] = true;
                    postas++;
                }

                if (ok)
                {
                    marcas[pos] = v;
                    Passo(pos + 1);
                }

                // Desfaz só o que ESTE nó marcou. Contar as postas não é zelo: a
                // distância que fez o nó falhar já estava marcada por um nível
                // acima, e desmarcá-la junto solta uma colisão que ninguém mais vê.
                for (var i = 0; i < postas; i++) usada[v - marcas[i]] = false;
            }
        }

        Passo(1);
        return new Achado(ordem, comprimento, achadas, nos);
    }

    /// <summary>
    /// A menor régua com <paramref name="ordem"/> marcas: sobe o comprimento de um
    /// em um a partir da cota trivial e para no primeiro que tem solução. O número
    /// de nós somado é o custo de PROVAR o ótimo, não o de achar uma régua.
    /// </summary>
    public static Achado Otima(int ordem, bool podaEspelho = true, IReadOnlyList<int>? minimos = null)
    {
        if (ordem < 1) throw new ArgumentOutOfRangeException(nameof(ordem));
        if (ordem == 1) return new Achado(1, 0, new List<Regua> { new(0) }, 0);

        long nos = 0;
        for (var comprimento = Cotas.Trivial(ordem); ; comprimento++)
        {
            var achado = Reguas(ordem, comprimento, podaEspelho, minimos);
            nos += achado.Nos;
            if (achado.Existe) return achado with { Nos = nos };
        }
    }

    /// <summary>
    /// Os comprimentos ótimos de 1 até <paramref name="ate"/> marcas, cada ordem
    /// usando como poda o que as ordens menores já provaram.
    /// </summary>
    public static int[] Otimos(int ate)
    {
        var minimos = new int[ate + 1];
        for (var n = 2; n <= ate; n++)
            minimos[n] = Otima(n, podaEspelho: true, minimos: minimos).Comprimento;
        return minimos;
    }
}
