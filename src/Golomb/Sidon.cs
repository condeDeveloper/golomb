namespace Conde.Golomb;

/// <summary>
/// A mesma ideia num anel: um conjunto de Sidon módulo m é um conjunto em que
/// nenhuma diferença CIRCULAR se repete. Toda régua tirada de um conjunto de
/// Sidon é de Golomb, e a volta não vale: é o anel que cobra mais caro.
/// </summary>
public static class Sidon
{
    /// <summary>
    /// Diferenças circulares: quantas vezes cada resto de 1 a m-1 aparece como
    /// diferença de dois elementos do conjunto, na ordem em que der.
    /// </summary>
    public static int[] Diferencas(IReadOnlyList<int> conjunto, int modulo)
    {
        ArgumentNullException.ThrowIfNull(conjunto);
        if (modulo < 1) throw new ArgumentOutOfRangeException(nameof(modulo));

        var conta = new int[modulo];
        foreach (var a in conjunto)
            foreach (var b in conjunto)
            {
                if (a == b) continue;
                var d = ((a - b) % modulo + modulo) % modulo;
                conta[d]++;
            }
        return conta;
    }

    /// <summary>Nenhuma diferença circular se repete.</summary>
    public static bool EhSidon(IReadOnlyList<int> conjunto, int modulo)
    {
        var conta = Diferencas(conjunto, modulo);
        for (var d = 1; d < modulo; d++)
            if (conta[d] > 1)
                return false;
        return true;
    }

    /// <summary>
    /// Conjunto perfeito de diferenças: cada resto de 1 a m-1 aparece EXATAMENTE
    /// uma vez. Com k elementos isso exige m = k(k-1) + 1, porque são k(k-1)
    /// diferenças ordenadas para cobrir m-1 restos.
    /// </summary>
    public static bool EhPerfeito(IReadOnlyList<int> conjunto, int modulo)
    {
        var conta = Diferencas(conjunto, modulo);
        for (var d = 1; d < modulo; d++)
            if (conta[d] != 1)
                return false;
        return true;
    }

    /// <summary>
    /// Todos os conjuntos perfeitos de diferenças com k elementos módulo
    /// k(k-1)+1, achados por varredura, normalizados para começar em 0. Dois
    /// conjuntos que são o mesmo girado aparecem uma vez só.
    /// </summary>
    public static List<int[]> Perfeitos(int elementos)
    {
        if (elementos < 2) throw new ArgumentOutOfRangeException(nameof(elementos));
        var modulo = elementos * (elementos - 1) + 1;
        var achados = new List<int[]>();
        var atual = new int[elementos];
        atual[0] = 0;
        var usada = new bool[modulo];

        void Passo(int pos)
        {
            if (pos == elementos)
            {
                achados.Add((int[])atual.Clone());
                return;
            }

            for (var v = atual[pos - 1] + 1; v < modulo; v++)
            {
                var ok = true;
                var postas = 0;
                for (var i = 0; i < pos; i++)
                {
                    var d1 = ((v - atual[i]) % modulo + modulo) % modulo;
                    var d2 = modulo - d1;
                    if (usada[d1] || usada[d2] || d1 == d2) { ok = false; break; }
                    usada[d1] = true;
                    usada[d2] = true;
                    postas++;
                }

                if (ok)
                {
                    atual[pos] = v;
                    Passo(pos + 1);
                }

                for (var i = 0; i < postas; i++)
                {
                    var d1 = ((v - atual[i]) % modulo + modulo) % modulo;
                    usada[d1] = false;
                    usada[modulo - d1] = false;
                }
            }
        }

        Passo(1);
        return achados;
    }

    /// <summary>
    /// A construção de Erdős e Turán, de 1941: para q primo, os números
    /// 2qk + (k² mod q) formam um conjunto de Sidon módulo 2q². Ela dá uma régua
    /// de q marcas sem procurar nada, e o preço dessa pressa está medido.
    /// </summary>
    public static int[] ErdosTuran(int primo)
    {
        if (!EhPrimo(primo)) throw new ArgumentException("a construção pede um primo: " + primo, nameof(primo));
        var saida = new int[primo];
        for (var k = 0; k < primo; k++)
            saida[k] = 2 * primo * k + k * k % primo;
        return saida;
    }

    /// <summary>
    /// O guloso de Mian e Chowla: começa em 0 e vai pegando sempre o menor inteiro
    /// que ainda não estraga nenhuma distância. Nunca volta atrás, e é por isso
    /// que ele serve de régua: ele mostra o que custa não voltar atrás.
    /// </summary>
    public static int[] MianChowla(int marcas)
    {
        if (marcas < 1) throw new ArgumentOutOfRangeException(nameof(marcas));
        var escolhidas = new List<int> { 0 };
        var usada = new HashSet<int>();

        while (escolhidas.Count < marcas)
        {
            for (var v = escolhidas[^1] + 1; ; v++)
            {
                var novas = new List<int>();
                var ok = true;
                foreach (var m in escolhidas)
                {
                    var d = v - m;
                    if (usada.Contains(d) || novas.Contains(d)) { ok = false; break; }
                    novas.Add(d);
                }

                if (!ok) continue;
                escolhidas.Add(v);
                foreach (var d in novas) usada.Add(d);
                break;
            }
        }

        return escolhidas.ToArray();
    }

    internal static bool EhPrimo(int n)
    {
        if (n < 2) return false;
        for (var d = 2; (long)d * d <= n; d++)
            if (n % d == 0)
                return false;
        return true;
    }
}
