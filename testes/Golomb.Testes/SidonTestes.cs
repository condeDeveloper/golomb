using Conde.Golomb;
using Xunit;

namespace Conde.Golomb.Testes;

public class SidonTestes
{
    [Fact]
    public void OConjuntoDeSeteComTresElementosEhPerfeito()
    {
        var conjunto = new[] { 0, 1, 3 };
        Assert.True(Sidon.EhSidon(conjunto, 7));
        Assert.True(Sidon.EhPerfeito(conjunto, 7));
    }

    [Fact]
    public void NoAnelAVoltaContaComoDistancia()
    {
        // Em linha {0,1,3} é régua de Golomb com qualquer módulo; no anel de 5 as
        // diferenças 3 e -3 = 2 colidem com 2 e 3.
        Assert.True(Juiz.EhGolomb(new Regua(0, 1, 3)));
        Assert.False(Sidon.EhSidon(new[] { 0, 1, 3 }, 5));
    }

    [Fact]
    public void ConjuntoPerfeitoMedeCadaRestoUmaVezSo()
    {
        var conta = Sidon.Diferencas(new[] { 0, 1, 3 }, 7);
        for (var d = 1; d < 7; d++) Assert.Equal(1, conta[d]);
    }

    [Fact]
    public void OModuloPrecisaDeSerKVezesKMenosUmMaisUm()
    {
        // Com 3 elementos só pode haver conjunto perfeito módulo 7.
        Assert.False(Sidon.EhPerfeito(new[] { 0, 1, 3 }, 8));
    }

    [Theory]
    [InlineData(3, 7, 6)]
    [InlineData(4, 13, 16)]
    [InlineData(5, 21, 10)]
    [InlineData(6, 31, 60)]
    public void QuantosConjuntosPerfeitosExistem(int elementos, int modulo, int quantos)
    {
        var achados = Sidon.Perfeitos(elementos);
        Assert.Equal(quantos, achados.Count);
        foreach (var conjunto in achados)
        {
            Assert.Equal(elementos, conjunto.Length);
            Assert.True(Sidon.EhPerfeito(conjunto, modulo));
        }
    }

    [Fact]
    public void ComSeteElementosNaoExisteNenhum()
    {
        // Seria um plano projetivo de ordem 6, e ele não existe: é o problema dos
        // 36 oficiais de Euler, fechado por Tarry em 1901.
        Assert.Empty(Sidon.Perfeitos(7));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(11)]
    public void ErdosTuranDaUmConjuntoDeSidonModuloDoisQQuadrado(int primo)
    {
        var conjunto = Sidon.ErdosTuran(primo);
        Assert.Equal(primo, conjunto.Length);
        Assert.True(Sidon.EhSidon(conjunto, 2 * primo * primo));
    }

    [Fact]
    public void ErdosTuranSoAceitaPrimo()
    {
        Assert.Throws<ArgumentException>(() => Sidon.ErdosTuran(9));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(11)]
    public void OGulosoDeMianChowlaSempreEntregaReguaDeGolomb(int marcas)
    {
        var regua = new Regua(Sidon.MianChowla(marcas));
        Assert.Equal(marcas, regua.Ordem);
        Assert.True(Juiz.EhGolomb(regua));
    }

    [Fact]
    public void OGulosoComecaComOsPrimeirosDaSequencia()
    {
        Assert.Equal(new[] { 0, 1, 3, 7, 12, 20, 30, 44, 65, 80, 96 }, Sidon.MianChowla(11));
    }

    [Fact]
    public void TodaConstrucaoPassaPeloJuizEnquantoNenhumaAcertaOMinimo()
    {
        foreach (var primo in new[] { 5, 7, 11 })
        {
            var guloso = new Regua(Sidon.MianChowla(primo));
            var erdos = new Regua(Sidon.ErdosTuran(primo));
            Assert.True(Juiz.EhGolomb(guloso));
            Assert.True(Juiz.EhGolomb(erdos));
            Assert.True(guloso.Comprimento > Medidas.Otimos[primo]);
            Assert.True(erdos.Comprimento > guloso.Comprimento);
        }
    }

    [Fact]
    public void CortarOAnelNumPontoDaUmaReguaDeGolomb()
    {
        foreach (var conjunto in Sidon.Perfeitos(5))
        {
            const int modulo = 21;
            foreach (var corte in conjunto)
            {
                var marcas = conjunto.Select(x => ((x - corte) % modulo + modulo) % modulo).ToArray();
                Assert.True(Juiz.EhGolomb(new Regua(marcas)));
            }
        }
    }
}
