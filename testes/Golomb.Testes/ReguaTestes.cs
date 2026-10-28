using Conde.Golomb;
using Xunit;

namespace Conde.Golomb.Testes;

public class ReguaTestes
{
    [Fact]
    public void AsMarcasSaemEmOrdemMesmoEntrandoBaguncadas()
    {
        var regua = new Regua(6, 0, 4, 1);
        Assert.Equal(new[] { 0, 1, 4, 6 }, regua.Marcas);
    }

    [Fact]
    public void AReguaPrecisaComecarEmZero()
    {
        var erro = Assert.Throws<ArgumentException>(() => new Regua(1, 2, 5));
        Assert.Contains("0", erro.Message);
    }

    [Fact]
    public void MarcaRepetidaNaoEhRegua()
    {
        Assert.Throws<ArgumentException>(() => new Regua(0, 3, 3, 7));
    }

    [Fact]
    public void ReguaSemMarcaNenhumaNaoExiste()
    {
        Assert.Throws<ArgumentException>(() => new Regua());
    }

    [Fact]
    public void OComprimentoEhAUltimaMarca()
    {
        Assert.Equal(6, new Regua(0, 1, 4, 6).Comprimento);
    }

    [Fact]
    public void SaoNVezesNMenosUmSobreDoisDistancias()
    {
        var regua = new Regua(0, 1, 4, 9, 11);
        Assert.Equal(10, regua.Distancias().Length);
    }

    [Fact]
    public void AsDistanciasDaReguaPerfeitaDeQuatroMarcasSaoUmAteSeis()
    {
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, new Regua(0, 1, 4, 6).Distancias());
    }

    [Fact]
    public void OEspelhoMedeExatamenteAsMesmasDistancias()
    {
        var regua = new Regua(0, 1, 4, 9, 11);
        Assert.Equal(regua.Distancias(), regua.Espelho().Distancias());
    }

    [Fact]
    public void OEspelhoDoEspelhoEhAReguaDeVolta()
    {
        var regua = new Regua(0, 2, 7, 8, 11);
        Assert.Equal(regua, regua.Espelho().Espelho());
    }

    [Fact]
    public void AReguaEOEspelhoTemAMesmaCanonica()
    {
        var regua = new Regua(0, 2, 7, 8, 11);
        Assert.Equal(regua.Canonica(), regua.Espelho().Canonica());
    }

    [Fact]
    public void DuasReguasComAsMesmasMarcasSaoIguais()
    {
        Assert.Equal(new Regua(0, 1, 4, 6), new Regua(0, 1, 4, 6));
        Assert.Equal(new Regua(0, 1, 4, 6).GetHashCode(), new Regua(6, 4, 1, 0).GetHashCode());
    }

    [Fact]
    public void ReguasDiferentesNaoSaoIguais()
    {
        Assert.NotEqual(new Regua(0, 1, 4, 6), new Regua(0, 2, 5, 6));
    }
}
