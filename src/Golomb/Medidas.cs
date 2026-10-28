using System.Diagnostics;

namespace Conde.Golomb;

/// <summary>
/// As medidas do README moram aqui, e não na suíte de testes. Os testes conferem
/// o que estes métodos devolvem; o medidor só imprime. Assim o número que está
/// escrito no README e o número que o teste exige são o mesmo número.
/// </summary>
public static class Medidas
{
    /// <summary>Até onde a varredura completa roda nas tabelas do medidor.</summary>
    public const int MaiorOrdem = 10;

    public readonly record struct LinhaOtima(
        int Ordem, int Trivial, int Comprimento, int Excesso, double Razao, int Canonicas, long Nos);

    public readonly record struct LinhaOnze(
        int Ordem, int Comprimento, int Canonicas, long Nos, long NosSoNoOtimo, double Segundos);

    public readonly record struct LinhaPoda(
        int Ordem, int Comprimento, long ComCota, long SemCota, long NosEspelho, int Canonicas, int Todas);

    public readonly record struct LinhaPerfeita(
        int Ordem, int Comprimento, int Perfeitas, int Mede, int Possiveis, double Cobertura);

    public readonly record struct LinhaJuiz(
        int Ordem, int Comprimento, bool BuscaDiz, bool JuizDiz, long NosBusca, double SegundosJuiz);

    public readonly record struct LinhaConstrucao(
        int Ordem, int Otimo, int Guloso, int ErdosTuran, double GulosoVezes, double ErdosTuranVezes);

    public readonly record struct LinhaAnel(
        int Elementos, int Modulo, int Perfeitos, int MenorComprimento, int OtimoLinear, bool Bate);

    /// <summary>
    /// Os comprimentos ótimos, indexados pelo número de marcas. Estão escritos
    /// aqui porque toda tabela usa esse valor de poda, e recalcular a varredura
    /// inteira em cada tabela custaria minutos sem mudar um dígito. Quem os PROVA
    /// é <see cref="PorOrdem"/>, que roda a busca e compara com esta lista, e
    /// <see cref="Onze"/>, que faz o mesmo com a ordem 11 sozinha. Se a busca
    /// mudar de resposta, o teste quebra aqui.
    /// </summary>
    public static readonly int[] Otimos = [0, 0, 1, 3, 6, 11, 17, 25, 34, 44, 55, 72];

    /// <summary>A tabela principal: o ótimo de cada ordem e o que custou prová-lo.</summary>
    public static List<LinhaOtima> PorOrdem()
    {
        var linhas = new List<LinhaOtima>();
        var minimos = new int[MaiorOrdem + 1];
        for (var n = 2; n <= MaiorOrdem; n++)
        {
            var achado = Busca.Otima(n, podaEspelho: true, minimos: minimos);
            minimos[n] = achado.Comprimento;
            linhas.Add(new LinhaOtima(
                n,
                Cotas.Trivial(n),
                achado.Comprimento,
                Cotas.Excesso(n, achado.Comprimento),
                Cotas.Razao(n, achado.Comprimento),
                achado.Reguas.Count,
                achado.Nos));
        }
        return linhas;
    }

    /// <summary>
    /// A ordem 11 sozinha, porque ela custa mais do que as nove ordens anteriores
    /// somadas e por isso não cabe na tabela de cima. Prova o 72 do mesmo jeito:
    /// varre todo comprimento de 55 a 72 e mostra que só o último tem régua.
    /// </summary>
    public static LinhaOnze Onze()
    {
        var minimos = new int[11];
        for (var n = 2; n <= 10; n++) minimos[n] = Otimos[n];

        var relogio = Stopwatch.StartNew();
        var achado = Busca.Otima(11, podaEspelho: true, minimos: minimos);
        relogio.Stop();
        var soNoOtimo = Busca.Reguas(11, achado.Comprimento, podaEspelho: true, minimos: minimos).Nos;

        return new LinhaOnze(11, achado.Comprimento, achado.Reguas.Count, achado.Nos, soNoOtimo, relogio.Elapsed.TotalSeconds);
    }

    /// <summary>
    /// O que cada poda vale. A de cota usa o ótimo já provado das ordens menores
    /// para saber que o pedaço que sobra não cabe. A de espelho fica com uma régua
    /// de cada par {régua, espelho} — e não corta nó nenhum, o que está medido aqui
    /// em vez de suposto.
    /// </summary>
    public static List<LinhaPoda> Podas()
    {
        var linhas = new List<LinhaPoda>();
        var minimos = Otimos;
        for (var n = 2; n <= MaiorOrdem; n++)
        {
            var l = minimos[n];
            var comCota = Busca.Reguas(n, l, podaEspelho: true, minimos: minimos);
            var semCota = Busca.Reguas(n, l, podaEspelho: true, minimos: null);
            var semEspelho = Busca.Reguas(n, l, podaEspelho: false, minimos: minimos);
            linhas.Add(new LinhaPoda(
                n, l, comCota.Nos, semCota.Nos, semEspelho.Nos, comCota.Reguas.Count, semEspelho.Reguas.Count));
        }
        return linhas;
    }

    /// <summary>
    /// Réguas perfeitas, as que medem toda distância de 1 ao comprimento sem
    /// buraco, e a cobertura das réguas ótimas que não são perfeitas.
    /// </summary>
    public static List<LinhaPerfeita> Perfeitas()
    {
        var linhas = new List<LinhaPerfeita>();
        var minimos = Otimos;
        for (var n = 2; n <= 9; n++)
        {
            var comprimento = Cotas.Trivial(n);
            var candidatas = Busca.Reguas(n, comprimento, podaEspelho: true, minimos: minimos);
            var perfeitas = candidatas.Reguas.Count(Juiz.EhPerfeita);

            var otima = Busca.Reguas(n, minimos[n], podaEspelho: true, minimos: minimos).Reguas[0];
            var mede = Juiz.QuantasMede(otima, otima.Comprimento);
            linhas.Add(new LinhaPerfeita(
                n, minimos[n], perfeitas, mede, otima.Comprimento, (double)mede / otima.Comprimento));
        }
        return linhas;
    }

    /// <summary>
    /// A busca contra o juiz que não sabe podar: para cada ordem pequena, o juiz
    /// refaz do zero a afirmação "não existe régua menor", sem bitset, sem cota,
    /// sem espelho, testando toda combinação de marcas.
    /// </summary>
    public static List<LinhaJuiz> ContraOJuiz()
    {
        var linhas = new List<LinhaJuiz>();
        var minimos = Otimos;
        for (var n = 2; n <= 8; n++)
        {
            var l = minimos[n];
            var achado = Busca.Reguas(n, l, podaEspelho: true, minimos: minimos);
            var relogio = Stopwatch.StartNew();
            var juiz = Juiz.NaoExisteMenor(n, l);
            relogio.Stop();
            linhas.Add(new LinhaJuiz(n, l, achado.Existe, juiz, achado.Nos, relogio.Elapsed.TotalSeconds));
        }
        return linhas;
    }

    /// <summary>
    /// As duas construções que não procuram contra a varredura que procura. O
    /// guloso de Mian-Chowla nunca volta atrás; Erdős-Turán nem olha para o que já
    /// colocou. As duas entregam uma régua de Golomb de verdade, e nenhuma das
    /// duas entrega a menor.
    /// </summary>
    public static List<LinhaConstrucao> Construcoes()
    {
        var linhas = new List<LinhaConstrucao>();
        var minimos = Otimos;
        foreach (var n in new[] { 2, 3, 5, 7, 11 })
        {
            var guloso = new Regua(Sidon.MianChowla(n)).Comprimento;
            var erdos = new Regua(Sidon.ErdosTuran(n)).Comprimento;
            var otimo = minimos[n];
            linhas.Add(new LinhaConstrucao(
                n, otimo, guloso, erdos, (double)guloso / otimo, (double)erdos / otimo));
        }
        return linhas;
    }

    /// <summary>
    /// O mesmo problema no anel. Um conjunto perfeito de diferenças com k
    /// elementos mede cada resto exatamente uma vez módulo k(k-1)+1, e cortar o
    /// anel num ponto dá uma régua de Golomb de brinde.
    /// </summary>
    public static List<LinhaAnel> NoAnel()
    {
        var linhas = new List<LinhaAnel>();
        var minimos = Otimos;
        for (var k = 3; k <= 8; k++)
        {
            var modulo = k * (k - 1) + 1;
            var perfeitos = Sidon.Perfeitos(k);
            var menor = int.MaxValue;
            foreach (var conjunto in perfeitos)
                foreach (var corte in conjunto)
                {
                    var marcas = conjunto.Select(x => ((x - corte) % modulo + modulo) % modulo).ToArray();
                    var regua = new Regua(marcas);
                    if (Juiz.EhGolomb(regua)) menor = Math.Min(menor, regua.Comprimento);
                }

            if (perfeitos.Count == 0) menor = 0;
            linhas.Add(new LinhaAnel(k, modulo, perfeitos.Count, menor, minimos[k], menor == minimos[k]));
        }
        return linhas;
    }
}
