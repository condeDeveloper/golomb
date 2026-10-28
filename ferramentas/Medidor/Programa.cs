using Conde.Golomb;

var comando = args.Length > 0 ? args[0] : "tudo";

if (comando is "otimos" or "tudo") Otimos();
if (comando is "onze" or "tudo") Onze();
if (comando is "podas" or "tudo") Podas();
if (comando is "perfeitas" or "tudo") Perfeitas();
if (comando is "juiz" or "tudo") ContraOJuiz();
if (comando is "construcoes" or "tudo") Construcoes();
if (comando is "anel" or "tudo") Anel();

static void Otimos()
{
    Console.WriteLine();
    Console.WriteLine("A MENOR REGUA DE CADA ORDEM, e o que custou provar que nao ha menor.");
    Console.WriteLine();
    Console.WriteLine($"  {"marcas",6} {"cota",6} {"otimo",6} {"excesso",8} {"L/n2",7} {"reguas",7} {"nos",14}");

    foreach (var l in Medidas.PorOrdem())
        Console.WriteLine($"  {l.Ordem,6} {l.Trivial,6} {l.Comprimento,6} {l.Excesso,8} {l.Razao,7:0.000} {l.Canonicas,7} {l.Nos,14:n0}");

    Console.WriteLine();
}

static void Onze()
{
    Console.WriteLine();
    Console.WriteLine("A ORDEM 11: provar que 72 e o minimo custa mais que as nove ordens anteriores juntas.");
    Console.WriteLine();
    var l = Medidas.Onze();
    Console.WriteLine($"  marcas            {l.Ordem}");
    Console.WriteLine($"  menor comprimento {l.Comprimento}");
    Console.WriteLine($"  reguas otimas     {l.Canonicas} (a menos de espelho)");
    Console.WriteLine($"  nos de 55 a 72    {l.Nos,16:n0}");
    Console.WriteLine($"  nos so em 72      {l.NosSoNoOtimo,16:n0}");
    Console.WriteLine($"  segundos          {l.Segundos,16:0.0}");
    Console.WriteLine();
}

static void Podas()
{
    Console.WriteLine();
    Console.WriteLine("O QUE CADA PODA VALE. A de cota corta arvore; a de espelho nao corta nada.");
    Console.WriteLine();
    Console.WriteLine($"  {"marcas",6} {"otimo",6} {"com cota",13} {"sem cota",14} {"corte",7} {"sem espelho",13} {"canonicas",10} {"todas",7}");

    foreach (var l in Medidas.Podas())
    {
        var corte = l.SemCota == 0 ? 0 : 100.0 * (l.SemCota - l.ComCota) / l.SemCota;
        Console.WriteLine($"  {l.Ordem,6} {l.Comprimento,6} {l.ComCota,13:n0} {l.SemCota,14:n0} {corte,6:0.0}% {l.NosEspelho,13:n0} {l.Canonicas,10} {l.Todas,7}");
    }

    Console.WriteLine();
}

static void Perfeitas()
{
    Console.WriteLine();
    Console.WriteLine("REGUA PERFEITA mede toda distancia de 1 ao comprimento. Elas acabam cedo.");
    Console.WriteLine();
    Console.WriteLine($"  {"marcas",6} {"otimo",6} {"perfeitas",10} {"mede",6} {"de",6} {"cobertura",10}");

    foreach (var l in Medidas.Perfeitas())
        Console.WriteLine($"  {l.Ordem,6} {l.Comprimento,6} {l.Perfeitas,10} {l.Mede,6} {l.Possiveis,6} {l.Cobertura,9:0.0%}");

    Console.WriteLine();
}

static void ContraOJuiz()
{
    Console.WriteLine();
    Console.WriteLine("A BUSCA CONTRA O JUIZ que nao poda nada: as duas tem de dizer a mesma coisa.");
    Console.WriteLine();
    Console.WriteLine($"  {"marcas",6} {"otimo",6} {"busca acha",11} {"juiz confirma",14} {"nos da busca",13} {"s do juiz",10}");

    foreach (var l in Medidas.ContraOJuiz())
        Console.WriteLine($"  {l.Ordem,6} {l.Comprimento,6} {(l.BuscaDiz ? "sim" : "NAO"),11} {(l.JuizDiz ? "sim" : "NAO"),14} {l.NosBusca,13:n0} {l.SegundosJuiz,10:0.000}");

    Console.WriteLine();
}

static void Construcoes()
{
    Console.WriteLine();
    Console.WriteLine("QUEM NAO PROCURA tambem entrega regua de Golomb. So nao entrega a menor.");
    Console.WriteLine();
    Console.WriteLine($"  {"marcas",6} {"otimo",6} {"guloso",7} {"Erdos-Turan",12} {"guloso x",9} {"E-T x",8}");

    foreach (var l in Medidas.Construcoes())
        Console.WriteLine($"  {l.Ordem,6} {l.Otimo,6} {l.Guloso,7} {l.ErdosTuran,12} {l.GulosoVezes,8:0.00}x {l.ErdosTuranVezes,7:0.00}x");

    Console.WriteLine();
}

static void Anel()
{
    Console.WriteLine();
    Console.WriteLine("NO ANEL: conjunto perfeito de diferencas mede cada resto exatamente uma vez.");
    Console.WriteLine();
    Console.WriteLine($"  {"elementos",10} {"modulo",7} {"perfeitos",10} {"cortado da",11} {"otimo linear",13} {"bate",6}");

    foreach (var l in Medidas.NoAnel())
        Console.WriteLine($"  {l.Elementos,10} {l.Modulo,7} {l.Perfeitos,10} {l.MenorComprimento,11} {l.OtimoLinear,13} {(l.Bate ? "sim" : "nao"),6}");

    Console.WriteLine();
}
