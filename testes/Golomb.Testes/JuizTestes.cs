using Conde.Golomb;
using Xunit;

namespace Conde.Golomb.Testes;

public class JuizTestes
{
    [Fact]
    public void AReguaPerfeitaDeQuatroMarcasEhDeGolomb()
    {
        Assert.True(Juiz.EhGolomb(new Regua(0, 1, 4, 6)));
    }

    [Fact]
    public void AReguaComDuasDistanciasIguaisNaoEh()
    {
        // 0-1 e 4-5 medem 1 os dois.
        Assert.False(Juiz.EhGolomb(new Regua(0, 1, 4, 5)));
    }

    [Fact]
    public void AColisaoVemComOsQuatroNumerosQueACausaram()
    {
        // 0-4 e 1-5 medem 4 os dois, e essa é a colisão que aparece primeiro na
        // varredura dos pares, antes da de distância 1 entre 0-1 e 4-5.
        var colisao = Juiz.PrimeiraColisao(new Regua(0, 1, 4, 5));
        Assert.NotNull(colisao);
        Assert.Equal(4, colisao!.Value.distancia);
        Assert.Equal((0, 4, 1, 5), (colisao.Value.a, colisao.Value.b, colisao.Value.c, colisao.Value.d));
    }

    [Fact]
    public void ReguaDeGolombNaoTemColisaoNenhuma()
    {
        Assert.Null(Juiz.PrimeiraColisao(new Regua(0, 1, 4, 9, 11)));
    }

    [Fact]
    public void ReguaPerfeitaMedeTudoDeUmAoComprimento()
    {
        Assert.True(Juiz.EhPerfeita(new Regua(0, 1, 4, 6)));
        Assert.True(Juiz.EhPerfeita(new Regua(0, 1, 3)));
    }

    [Fact]
    public void AOtimaDeCincoMarcasNaoEhPerfeitaPorqueTemBuraco()
    {
        var otima = new Regua(0, 1, 4, 9, 11);
        Assert.True(Juiz.EhGolomb(otima));
        Assert.False(Juiz.EhPerfeita(otima));
        Assert.Equal(10, Juiz.QuantasMede(otima, 11));
    }

    [Fact]
    public void QuantasMedeNaoContaDistanciaRepetida()
    {
        // Não é de Golomb: o 4 aparece em três pares e o 8 em dois, então as dez
        // distâncias medem só sete valores diferentes.
        var regua = new Regua(0, 1, 4, 8, 12);
        Assert.Equal(10, regua.Distancias().Length);
        Assert.Equal(7, Juiz.QuantasMede(regua, 12));
    }

    [Fact]
    public void OJuizConfirmaQueNaoHaReguaDeCincoMarcasAbaixoDeOnze()
    {
        Assert.True(Juiz.NaoExisteMenor(5, 11));
    }

    [Fact]
    public void OJuizRecusaUmaAfirmacaoFalsaDeMinimo()
    {
        // Existe régua de 4 marcas com comprimento 6, então "o mínimo é 7" é mentira.
        Assert.False(Juiz.NaoExisteMenor(4, 7));
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 6)]
    [InlineData(5, 11)]
    [InlineData(6, 17)]
    [InlineData(7, 25)]
    public void OJuizConfirmaOMinimoDeCadaOrdemPequena(int ordem, int comprimento)
    {
        Assert.True(Juiz.NaoExisteMenor(ordem, comprimento));
        Assert.False(Juiz.NaoExisteMenor(ordem, comprimento + 1));
    }
}
