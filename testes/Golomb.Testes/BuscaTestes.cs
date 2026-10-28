using Conde.Golomb;
using Xunit;

namespace Conde.Golomb.Testes;

public class BuscaTestes
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 6)]
    [InlineData(5, 11)]
    [InlineData(6, 17)]
    [InlineData(7, 25)]
    [InlineData(8, 34)]
    public void AMenorReguaDeCadaOrdem(int ordem, int comprimento)
    {
        Assert.Equal(comprimento, Busca.Otima(ordem).Comprimento);
    }

    [Fact]
    public void TodaReguaAchadaPassaPeloJuiz()
    {
        for (var ordem = 2; ordem <= 8; ordem++)
        {
            var achado = Busca.Otima(ordem);
            Assert.NotEmpty(achado.Reguas);
            foreach (var regua in achado.Reguas)
            {
                Assert.True(Juiz.EhGolomb(regua), $"{regua} não é régua de Golomb");
                Assert.Equal(ordem, regua.Ordem);
                Assert.Equal(achado.Comprimento, regua.Comprimento);
            }
        }
    }

    [Fact]
    public void NaoExisteReguaUmPassoAbaixoDoOtimo()
    {
        for (var ordem = 2; ordem <= 9; ordem++)
        {
            var otimo = Medidas.Otimos[ordem];
            Assert.False(Busca.Reguas(ordem, otimo - 1).Existe, $"ordem {ordem}");
        }
    }

    [Fact]
    public void ADuasDeCincoMarcasSaoAsDoLivro()
    {
        var achado = Busca.Reguas(5, 11);
        var texto = achado.Reguas.Select(r => r.ToString()).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[] { "0 1 4 9 11", "0 2 7 8 11" }, texto);
    }

    [Fact]
    public void SemAPodaDeEspelhoCadaReguaApareceDuasVezes()
    {
        for (var ordem = 3; ordem <= 9; ordem++)
        {
            var comprimento = Medidas.Otimos[ordem];
            var canonicas = Busca.Reguas(ordem, comprimento).Reguas;
            var todas = Busca.Reguas(ordem, comprimento, podaEspelho: false).Reguas;

            Assert.Equal(2 * canonicas.Count, todas.Count);
            Assert.Equal(canonicas.Count, todas.Select(r => r.Canonica()).Distinct().Count());
        }
    }

    [Fact]
    public void AReguaDeGolombNuncaEhSimetrica()
    {
        // É o que faz a poda de espelho ser exata: se houvesse régua igual ao
        // próprio espelho, ela seria jogada fora sem ter par.
        for (var ordem = 2; ordem <= 9; ordem++)
            foreach (var regua in Busca.Reguas(ordem, Medidas.Otimos[ordem], podaEspelho: false).Reguas)
                if (ordem > 2)
                    Assert.NotEqual(regua, regua.Espelho());
    }

    [Fact]
    public void APodaDeEspelhoNaoCortaUmNoSequer()
    {
        for (var ordem = 2; ordem <= 9; ordem++)
        {
            var comprimento = Medidas.Otimos[ordem];
            var com = Busca.Reguas(ordem, comprimento);
            var sem = Busca.Reguas(ordem, comprimento, podaEspelho: false);
            Assert.Equal(sem.Nos, com.Nos);
        }
    }

    [Fact]
    public void APodaDeCotaNaoMudaOQueEhAchado()
    {
        for (var ordem = 2; ordem <= 8; ordem++)
        {
            var comprimento = Medidas.Otimos[ordem];
            var com = Busca.Reguas(ordem, comprimento, minimos: Medidas.Otimos);
            var sem = Busca.Reguas(ordem, comprimento, minimos: null);
            Assert.Equal(
                com.Reguas.Select(r => r.ToString()).OrderBy(x => x, StringComparer.Ordinal),
                sem.Reguas.Select(r => r.ToString()).OrderBy(x => x, StringComparer.Ordinal));
            Assert.True(com.Nos <= sem.Nos);
        }
    }

    [Fact]
    public void OsOtimosCalculadosEmSequenciaBatemComATabela()
    {
        var calculados = Busca.Otimos(9);
        for (var ordem = 2; ordem <= 9; ordem++)
            Assert.Equal(Medidas.Otimos[ordem], calculados[ordem]);
    }

    [Fact]
    public void AVarreduraEOJuizConcordamEmQuemEhOMinimo()
    {
        for (var ordem = 2; ordem <= 7; ordem++)
        {
            var otimo = Busca.Otima(ordem).Comprimento;
            Assert.True(Juiz.NaoExisteMenor(ordem, otimo));
        }
    }

    [Fact]
    public void OrdemZeroNaoEhPergunta()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Busca.Otima(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Busca.Reguas(3, -1));
    }
}
