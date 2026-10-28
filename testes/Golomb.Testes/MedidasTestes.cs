using Conde.Golomb;
using Xunit;

namespace Conde.Golomb.Testes;

/// <summary>
/// Todo número que está escrito no README está preso aqui. Se a busca mudar de
/// ideia sobre qualquer coisa, este arquivo é que quebra, e não o leitor.
/// </summary>
public class MedidasTestes
{
    [Fact]
    public void ATabelaDosOtimosTemUmaLinhaPorOrdem()
    {
        var linhas = Medidas.PorOrdem();
        Assert.Equal(9, linhas.Count);
        Assert.Equal(2, linhas[0].Ordem);
        Assert.Equal(Medidas.MaiorOrdem, linhas[^1].Ordem);
    }

    [Theory]
    [InlineData(2, 1, 1, 0, 1, 1L)]
    [InlineData(3, 3, 3, 0, 1, 4L)]
    [InlineData(4, 6, 6, 0, 1, 19L)]
    [InlineData(5, 10, 11, 1, 2, 259L)]
    [InlineData(6, 15, 17, 2, 4, 2_576L)]
    [InlineData(7, 21, 25, 4, 5, 35_354L)]
    [InlineData(8, 28, 34, 6, 1, 357_663L)]
    [InlineData(9, 36, 44, 8, 1, 3_005_446L)]
    [InlineData(10, 45, 55, 10, 1, 22_481_732L)]
    public void CadaLinhaDaTabelaPrincipal(int ordem, int trivial, int otimo, int excesso, int canonicas, long nos)
    {
        var linha = Medidas.PorOrdem().Single(l => l.Ordem == ordem);
        Assert.Equal(trivial, linha.Trivial);
        Assert.Equal(otimo, linha.Comprimento);
        Assert.Equal(excesso, linha.Excesso);
        Assert.Equal(canonicas, linha.Canonicas);
        Assert.Equal(nos, linha.Nos);
    }

    [Fact]
    public void ACotaDeContagemSoEhAlcancadaAteQuatroMarcas()
    {
        foreach (var linha in Medidas.PorOrdem())
            Assert.Equal(linha.Ordem <= 4, linha.Excesso == 0);
    }

    [Fact]
    public void ORazaoSobeDeZeroVirgulaDoisCincoACincoVirgulaCinco()
    {
        var linhas = Medidas.PorOrdem();
        Assert.Equal(0.250, linhas[0].Razao, 3);
        Assert.Equal(0.550, linhas[^1].Razao, 3);
        for (var i = 1; i < linhas.Count; i++)
            Assert.True(linhas[i].Razao > linhas[i - 1].Razao, $"ordem {linhas[i].Ordem}");
    }

    [Theory]
    [InlineData(4, 19L, 22L, 1, 2)]
    [InlineData(5, 156L, 194L, 2, 4)]
    [InlineData(6, 1_240L, 1_732L, 4, 8)]
    [InlineData(7, 13_169L, 19_492L, 5, 10)]
    [InlineData(8, 117_934L, 195_176L, 1, 2)]
    [InlineData(9, 913_233L, 1_779_350L, 1, 2)]
    [InlineData(10, 6_344_507L, 15_272_631L, 1, 2)]
    public void CadaLinhaDaTabelaDePodas(int ordem, long comCota, long semCota, int canonicas, int todas)
    {
        var linha = Medidas.Podas().Single(l => l.Ordem == ordem);
        Assert.Equal(comCota, linha.ComCota);
        Assert.Equal(semCota, linha.SemCota);
        Assert.Equal(canonicas, linha.Canonicas);
        Assert.Equal(todas, linha.Todas);
    }

    [Fact]
    public void APodaDeEspelhoCortaZeroNosEmTodaOrdem()
    {
        foreach (var linha in Medidas.Podas())
            Assert.Equal(linha.ComCota, linha.NosEspelho);
    }

    [Fact]
    public void APodaDeCotaCortaDeTrezeAtoCinquentaEOitoPorCento()
    {
        var linhas = Medidas.Podas().Where(l => l.Ordem >= 4).ToList();
        foreach (var linha in linhas)
        {
            var corte = 100.0 * (linha.SemCota - linha.ComCota) / linha.SemCota;
            Assert.InRange(corte, 13.0, 59.0);
        }
        Assert.Equal(13.6, 100.0 * (linhas[0].SemCota - linhas[0].ComCota) / linhas[0].SemCota, 1);
        Assert.Equal(58.5, 100.0 * (linhas[^1].SemCota - linhas[^1].ComCota) / linhas[^1].SemCota, 1);
    }

    [Fact]
    public void SemEspelhoSaoSempreOExatoDobro()
    {
        foreach (var linha in Medidas.Podas().Where(l => l.Ordem >= 3))
            Assert.Equal(2 * linha.Canonicas, linha.Todas);
    }

    [Theory]
    [InlineData(2, 1, 1, 1)]
    [InlineData(3, 1, 3, 3)]
    [InlineData(4, 1, 6, 6)]
    [InlineData(5, 0, 10, 11)]
    [InlineData(6, 0, 15, 17)]
    [InlineData(7, 0, 21, 25)]
    [InlineData(8, 0, 28, 34)]
    [InlineData(9, 0, 36, 44)]
    public void CadaLinhaDaTabelaDeReguasPerfeitas(int ordem, int perfeitas, int mede, int comprimento)
    {
        var linha = Medidas.Perfeitas().Single(l => l.Ordem == ordem);
        Assert.Equal(perfeitas, linha.Perfeitas);
        Assert.Equal(mede, linha.Mede);
        Assert.Equal(comprimento, linha.Comprimento);
    }

    [Fact]
    public void ReguaPerfeitaMorreEmQuatroMarcas()
    {
        foreach (var linha in Medidas.Perfeitas())
            Assert.Equal(linha.Ordem <= 4 ? 1 : 0, linha.Perfeitas);
    }

    [Fact]
    public void ACoberturaCaiDeCemPorCentoAOitentaEDois()
    {
        var linhas = Medidas.Perfeitas();
        Assert.Equal(1.000, linhas.First(l => l.Ordem == 4).Cobertura, 3);
        Assert.Equal(0.909, linhas.First(l => l.Ordem == 5).Cobertura, 3);
        Assert.Equal(0.818, linhas.First(l => l.Ordem == 9).Cobertura, 3);
    }

    [Fact]
    public void OJuizConfirmaTodaOrdemQueABuscaAfirma()
    {
        foreach (var linha in Medidas.ContraOJuiz())
        {
            Assert.True(linha.BuscaDiz, $"ordem {linha.Ordem}");
            Assert.True(linha.JuizDiz, $"ordem {linha.Ordem}");
            Assert.Equal(Medidas.Otimos[linha.Ordem], linha.Comprimento);
        }
    }

    [Theory]
    [InlineData(5, 1L, 156L)]
    [InlineData(6, 1L, 1_240L)]
    [InlineData(7, 1L, 13_169L)]
    [InlineData(8, 1L, 117_934L)]
    public void OsNosDaBuscaNoComprimentoOtimo(int ordem, long minimo, long nos)
    {
        var linha = Medidas.ContraOJuiz().Single(l => l.Ordem == ordem);
        Assert.True(linha.NosBusca >= minimo);
        Assert.Equal(nos, linha.NosBusca);
    }

    [Theory]
    [InlineData(2, 1, 1, 5)]
    [InlineData(3, 3, 3, 13)]
    [InlineData(5, 11, 12, 41)]
    [InlineData(7, 25, 30, 85)]
    [InlineData(11, 72, 96, 221)]
    public void CadaLinhaDasConstrucoes(int ordem, int otimo, int guloso, int erdosTuran)
    {
        var linha = Medidas.Construcoes().Single(l => l.Ordem == ordem);
        Assert.Equal(otimo, linha.Otimo);
        Assert.Equal(guloso, linha.Guloso);
        Assert.Equal(erdosTuran, linha.ErdosTuran);
    }

    [Fact]
    public void OGulosoPioraEErdosTuranMelhoraConformeAReguaCresce()
    {
        var linhas = Medidas.Construcoes();
        Assert.Equal(1.00, linhas.First(l => l.Ordem == 3).GulosoVezes, 2);
        Assert.Equal(1.33, linhas.First(l => l.Ordem == 11).GulosoVezes, 2);
        Assert.Equal(5.00, linhas.First(l => l.Ordem == 2).ErdosTuranVezes, 2);
        Assert.Equal(3.07, linhas.First(l => l.Ordem == 11).ErdosTuranVezes, 2);
    }

    [Theory]
    [InlineData(3, 7, 6, 3, 3, true)]
    [InlineData(4, 13, 16, 6, 6, true)]
    [InlineData(5, 21, 10, 11, 11, true)]
    [InlineData(6, 31, 60, 17, 17, true)]
    [InlineData(7, 43, 0, 0, 25, false)]
    [InlineData(8, 57, 96, 35, 34, false)]
    public void CadaLinhaDoAnel(int elementos, int modulo, int perfeitos, int cortada, int otimo, bool bate)
    {
        var linha = Medidas.NoAnel().Single(l => l.Elementos == elementos);
        Assert.Equal(modulo, linha.Modulo);
        Assert.Equal(perfeitos, linha.Perfeitos);
        Assert.Equal(cortada, linha.MenorComprimento);
        Assert.Equal(otimo, linha.OtimoLinear);
        Assert.Equal(bate, linha.Bate);
    }

    [Fact]
    public void OAnelEntregaOOtimoDeGracaAteSeisMarcasEDepoisNaoMais()
    {
        foreach (var linha in Medidas.NoAnel())
            Assert.Equal(linha.Elementos <= 6, linha.Bate);
    }

    [Fact]
    public void AOrdemOnzeCustaMaisQueTodasAsAnterioresSomadas()
    {
        var onze = Medidas.Onze();
        Assert.Equal(72, onze.Comprimento);
        Assert.Equal(2, onze.Canonicas);
        Assert.Equal(613_854_505L, onze.Nos);
        Assert.Equal(139_189_884L, onze.NosSoNoOtimo);

        var anteriores = Medidas.PorOrdem().Sum(l => l.Nos);
        Assert.True(onze.Nos > anteriores, $"{onze.Nos} contra {anteriores}");
        Assert.Equal(25_883_054L, anteriores);
    }

    [Fact]
    public void ATabelaDeOtimosEhAMesmaQueABuscaProva()
    {
        foreach (var linha in Medidas.PorOrdem())
            Assert.Equal(Medidas.Otimos[linha.Ordem], linha.Comprimento);
    }
}
