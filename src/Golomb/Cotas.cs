namespace Conde.Golomb;

/// <summary>
/// As contas que dizem alguma coisa sobre o comprimento sem procurar régua
/// nenhuma. Nenhuma delas acha uma régua: elas só impedem que a busca procure
/// onde provadamente não há.
/// </summary>
public static class Cotas
{
    /// <summary>
    /// A cota de contagem: com n marcas são n(n-1)/2 distâncias, todas diferentes
    /// e todas entre 1 e o comprimento, então o comprimento não pode ser menor que
    /// a quantidade de distâncias. Uma linha de conta que a varredura não precisa
    /// descobrir sozinha.
    /// </summary>
    public static int Trivial(int ordem)
    {
        if (ordem < 1) throw new ArgumentOutOfRangeException(nameof(ordem));
        return ordem * (ordem - 1) / 2;
    }

    /// <summary>
    /// Quanto o ótimo medido ficou acima da cota de contagem. É o preço de não
    /// conseguir encaixar todas as distâncias sem buraco.
    /// </summary>
    public static int Excesso(int ordem, int comprimentoOtimo) => comprimentoOtimo - Trivial(ordem);

    /// <summary>
    /// A razão entre o comprimento e o quadrado da ordem. Para régua grande esse
    /// número tende a 1, e é por isso que régua de Golomb é "quase" um conjunto de
    /// Sidon perfeito.
    /// </summary>
    public static double Razao(int ordem, int comprimento) => ordem <= 1 ? 0 : (double)comprimento / (ordem * ordem);
}
