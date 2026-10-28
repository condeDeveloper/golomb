# Réguas de Golomb

Réguas de Golomb do zero em C# e .NET 8: marcas num pedaço de madeira de modo que
duas marcas nunca meçam a mesma distância. A pergunta é sempre a mesma, com n
marcas, qual o menor pedaço de madeira, e a resposta é **varrida**, não citada.

A régua comum de 30 cm mede 1 cm de 29 jeitos diferentes. Isso é desperdício: se
cada par de marcas medisse um valor distinto, 4 marcas bastariam para medir 6
distâncias. É o que Sidon pediu em 1932 estudando séries de Fourier, o que Babcock
reinventou em 1953 posicionando antenas de rádio, e o que Golomb formalizou. Hoje
é o espaçamento dos receptores de um radiotelescópio e a escolha dos canais de uma
transmissão para que nenhuma interferência de terceira ordem caia em cima de outra.

```bash
dotnet test testes/Golomb.Testes/Golomb.Testes.csproj -c Release    # 120 testes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release  # todas as tabelas
```

## A menor régua de cada ordem

A busca monta a régua marca por marca e desiste assim que duas marcas medem a mesma
distância. Para provar o mínimo ela sobe o comprimento de um em um a partir da cota
de contagem e mostra que nenhum comprimento menor tem solução.

```
  marcas   cota  otimo  excesso    L/n2  reguas            nos
       2      1      1        0   0.250       1              1
       3      3      3        0   0.333       1              4
       4      6      6        0   0.375       1             19
       5     10     11        1   0.440       2            259
       6     15     17        2   0.472       4          2,576
       7     21     25        4   0.510       5         35,354
       8     28     34        6   0.531       1        357,663
       9     36     44        8   0.543       1      3,005,446
      10     45     55       10   0.550       1     22,481,732
```

A coluna `cota` é uma linha de conta: com n marcas são n(n-1)/2 distâncias, todas
diferentes e todas menores ou iguais ao comprimento, então o comprimento não pode
ser menor que a quantidade de distâncias. **A cota só é alcançada até 4 marcas.**
De 5 em diante sobra buraco, e o excesso vira 1, 2, 4, 6, 8, 10 e, na ordem 11, 17.

A ordem 11 não cabe na tabela porque custa mais que todas as outras somadas:

```
  marcas            11
  menor comprimento 72
  reguas otimas     2 (a menos de espelho)
  nos de 55 a 72         613,854,505
  nos so em 72           139,189,884
```

**613.854.505 nós contra 25.883.054 das nove ordens anteriores juntas**, 23 vezes
mais para uma marca a mais. E repare: só 139 milhões desses nós foram gastos no
comprimento que tem resposta. Os outros 474 milhões foram gastos provando que 55,
56, 57 e os outros dezesseis comprimentos não servem. **Achar a régua é a parte
barata; a parte cara é provar que não há menor.**

## A poda que eu achei que fosse poda

```
  marcas  otimo      com cota       sem cota   corte   sem espelho  canonicas   todas
       4      6            19             22   13.6%            19          1       2
       5     11           156            194   19.6%           156          2       4
       6     17         1,240          1,732   28.4%         1,240          4       8
       7     25        13,169         19,492   32.4%        13,169          5      10
       8     34       117,934        195,176   39.6%       117,934          1       2
       9     44       913,233      1,779,350   48.7%       913,233          1       2
      10     55     6,344,507     15,272,631   58.5%     6,344,507          1       2
```

Duas podas moram nesta busca e só uma é poda.

A **de cota** usa o ótimo já provado das ordens menores: se o que sobra do pedaço de
madeira já é menor que o mínimo das marcas que faltam, não adianta continuar. Ela
corta de 13,6% a 58,5% dos nós, e corta mais quanto maior a régua.

A **de espelho** fica com uma régua de cada par {régua, espelho}, porque virar a
régua do avesso não muda nada do que ela mede. Eu ia escrever aqui que ela corta
metade da árvore. **Ela corta zero nós, em todas as ordens, dígito por dígito.** A
coluna `sem espelho` é idêntica à coluna `com cota` em todas as linhas. O motivo é
que a comparação "a primeira distância é menor que a última" só pode ser feita
quando a última marca entra, isto é, depois de o trabalho ter sido feito. O que ela
faz é dividir a *lista de respostas* por dois, e isso ela faz exatamente: `todas` é
sempre o dobro de `canonicas`, sem sobra. **Numa régua de Golomb a primeira e a
última distância nunca são iguais**, afinal seriam duas distâncias iguais, e por
isso nenhuma régua é o próprio espelho e nenhuma fica sem par.

## Régua perfeita acaba em quatro marcas

Uma régua perfeita mede *todas* as distâncias de 1 ao comprimento, sem buraco
nenhum. A de 4 marcas é `0 1 4 6`, e ela mede 1, 2, 3, 4, 5 e 6.

```
  marcas  otimo  perfeitas   mede     de  cobertura
       2      1          1      1      1    100.0%
       3      3          1      3      3    100.0%
       4      6          1      6      6    100.0%
       5     11          0     10     11     90.9%
       6     17          0     15     17     88.2%
       7     25          0     21     25     84.0%
       8     34          0     28     34     82.4%
       9     44          0     36     44     81.8%
```

Com 5 marcas **não existe nenhuma**, e nunca mais existe. A varredura confirma o
que a aritmética já pedia: a régua ótima de 5 marcas tem comprimento 11 e só
consegue medir 10 valores, então um valor entre 1 e 11 fica de fora. A cobertura
cai de 100% para 81,8% em nove marcas, e continua caindo. **Medir muito e ser curta
são objetivos diferentes**, e a partir de 5 marcas eles brigam.

## A busca contra um juiz que não sabe nada

```
  marcas  otimo  busca acha  juiz confirma  nos da busca  s do juiz
       5     11         sim            sim           156      0.000
       6     17         sim            sim         1,240      0.002
       7     25         sim            sim        13,169      0.064
       8     34         sim            sim       117,934      1.822
```

O juiz não tem bitset, não tem cota, não tem espelho e não tem poda: ele monta
toda combinação de marcas possível, uma por uma, e pergunta à definição. É lento de
propósito, porque o que ele não pode é compartilhar um erro com a busca. Na ordem 8
ele leva 1,8 segundo para dizer o que a busca diz em 117.934 nós, e as duas dizem a
mesma coisa em todas as ordens que ele aguenta.

## Quem não procura também entrega régua

```
  marcas  otimo  guloso  Erdos-Turan  guloso x    E-T x
       2      1       1            5     1.00x    5.00x
       3      3       3           13     1.00x    4.33x
       5     11      12           41     1.09x    3.73x
       7     25      30           85     1.20x    3.40x
      11     72      96          221     1.33x    3.07x
```

O **guloso de Mian-Chowla** começa em 0 e pega sempre o menor inteiro que ainda não
estraga nenhuma distância: `0 1 3 7 12 20 30 44 65 80 96`. Nunca volta atrás, e
entrega uma régua de Golomb de verdade. Com 3 marcas ele acerta o ótimo; com 11 ele
já está 33% acima. **O preço de não voltar atrás cresce.**

A construção de **Erdős-Turán**, de 1941, é ainda mais apressada: para q primo, os
números 2qk + (k² mod q) formam um conjunto de Sidon módulo 2q². Ela não olha nem
para o que já colocou, sai numa passada, e por isso começa 5 vezes acima do ótimo.
Mas ela vai na direção contrária: 5,00x, 4,33x, 3,73x, 3,40x, 3,07x. **O guloso
piora e a fórmula melhora**, porque o que a fórmula garante é a ordem de grandeza, e
ordem de grandeza só aparece quando a régua cresce.

## O mesmo problema no anel

Num anel de m posições, a distância entre duas marcas pode ser medida pelos dois
lados. Um conjunto em que nenhuma diferença circular se repete é um **conjunto de
Sidon**; se cada resto de 1 a m-1 aparece *exatamente* uma vez, ele é **perfeito**, e
isso exige m = k(k-1)+1.

```
   elementos  modulo  perfeitos  cortado da  otimo linear   bate
           3       7          6           3             3    sim
           4      13         16           6             6    sim
           5      21         10          11            11    sim
           6      31         60          17            17    sim
           7      43          0           0            25    nao
           8      57         96          35            34    nao
```

Cortar o anel num ponto e esticar dá uma régua de Golomb de brinde. E de 3 a 6
elementos ela não é uma régua qualquer: **é exatamente a ótima**, de graça, sem
busca nenhuma. 3, 6, 11 e 17 são os mesmos números da primeira tabela.

Com 7 elementos **não existe conjunto perfeito nenhum**, e a varredura dos 43 restos
não acha um. Não é azar: um conjunto perfeito de diferenças com q+1 elementos é um
plano projetivo de ordem q, e o plano projetivo de ordem 6 não existe. É o problema
dos 36 oficiais que Euler levantou em 1782 e Tarry fechou em 1901 conferindo os
casos na mão. Aqui ele reaparece sem ser convidado, no meio de uma pergunta sobre
régua.

Com 8 elementos os conjuntos voltam, 96 deles, mas o melhor corte dá **35** contra os
34 da busca. **O anel entrega o ótimo de graça até 6 marcas, e só até lá.**

## O que as medidas me corrigiram

**A primeira tabela que saiu era bonita demais.** Excesso 0 em todas as linhas: o
ótimo batendo a cota de contagem em toda ordem, de 2 a 11. Deveria ter me ocorrido
na hora que, se fosse assim, não haveria problema nenhum para pesquisar, e sessenta
anos de gente procurando régua de 25 marcas não fariam sentido. O defeito era a
última marca: eu conferia a distância dela só contra a marca anterior, e não contra
todas as outras. Com isso toda régua "fechava" no comprimento mínimo. **Número que
confirma o que eu queria é o que merece a segunda conferida, não o que contraria.**

**O desfazer do bitset soltava distância que não era dele.** Ao testar uma marca
nova eu marcava as distâncias até achar uma colisão, e desfazia percorrendo de novo
e parando na primeira que não estivesse marcada. Só que a distância que causou a
colisão estava marcada *por um nível acima*: o desfazer a apagava junto e liberava
uma distância que continuava ocupada. A busca passou a aceitar régua inválida. A
correção é contar quantas foram marcadas e desfazer exatamente essas, e o teste que
pega isso é o que manda toda régua achada passar pelo juiz.

**Chamei de poda uma coisa que não poda.** A poda de espelho estava escrita,
comentada e medida como se cortasse metade do trabalho. Corta zero: a coluna de nós
com e sem ela é idêntica em todas as ordens. Eu teria escrito "corta metade" no
README sem piscar, porque *parece* que corta. O que ela corta é o tamanho da lista
de respostas, e só.

**O guloso empata com o ótimo e eu não esperava.** Em 2 e 3 marcas Mian-Chowla dá
exatamente a régua mínima. A conclusão fácil, "guloso é pior", precisou virar uma
frase com domínio: ele é pior *a partir de 5 marcas*, e a distância cresce.

**O juiz aponta a colisão que vem primeiro na varredura dos pares, não a de menor
distância.** Em `0 1 4 5` eu esperava que ele acusasse o 1, de 0-1 e 4-5. Ele acusa
o 4, de 0-4 e 1-5, porque é essa que a varredura encontra antes. O teste estava
errado, não o juiz.

## Os arquivos

| arquivo | o que faz |
|---|---|
| `src/Golomb/Regua.cs` | as marcas, sempre começando em 0, as distâncias, o espelho e a canônica |
| `src/Golomb/Juiz.cs` | a definição: nenhuma distância duas vezes. E o "não existe menor" refeito no braço |
| `src/Golomb/Busca.cs` | a varredura marca a marca, com bitset de distâncias, poda de cota e poda de espelho |
| `src/Golomb/Cotas.cs` | a cota de contagem, o excesso e a razão L/n² |
| `src/Golomb/Sidon.cs` | o anel: conjuntos de Sidon, conjuntos perfeitos, Erdős-Turán e o guloso de Mian-Chowla |
| `src/Golomb/Medidas.cs` | as seis tabelas deste README |
| `ferramentas/Medidor` | imprime as tabelas, uma por comando |

## Rodando

```bash
dotnet test testes/Golomb.Testes/Golomb.Testes.csproj -c Release

dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- otimos
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- onze
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- podas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- perfeitas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- construcoes
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- anel
```

C# 12 e .NET 8. Zero dependências fora do xUnit, e só nos testes.

120 testes. Os números deste README estão presos em `MedidasTestes`.
