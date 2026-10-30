# Quebra-Cabeça VR (Top-III)

Protótipo de quebra-cabeça em realidade virtual feito na Unity com o **XR Interaction Toolkit**. O jogador escolhe o tamanho do puzzle e a imagem, pega as peças com as mãos (controles VR) e encaixa cada uma no lugar certo. Quando todas as peças estão corretas, aparece a mensagem de vitória e toca um som.

Trabalho em grupo da disciplina **Tópicos 3**. Tipo de interação trabalhada: **manipulação**.

## Como funciona

1. **Menu em 3D:** na cena, o jogador escolhe primeiro o tamanho do puzzle (2 x 2 ou 3 x 3) e depois uma das imagens disponíveis.
2. **Montagem automática:** o puzzle é gerado em tempo de execução. A imagem é dividida em peças, que aparecem embaralhadas em uma mesa, na frente de um quadro.
3. **Jogo:** o jogador pega cada peça (*grab*) e encaixa no espaço correspondente do quadro (*socket*).
4. **Vitória:** quando todos os encaixes estão corretos, aparece "Muito bem! Puzzle completo!" e toca uma sequência de notas. Se o jogador tirar uma peça, a mensagem some.
5. **Reiniciar / trocar tamanho:** botões no menu permitem embaralhar de novo ou voltar à escolha de tamanho.

## Requisitos

| Item | Versão |
| --- | --- |
| Unity | 6000.3.0f1 (Unity 6.3 LTS) |
| XR Interaction Toolkit | 3.4.0 |
| OpenXR | 1.16.1 |
| Render pipeline | Universal Render Pipeline (URP) |

Os pacotes são instalados automaticamente pela Unity a partir de `Packages/manifest.json`.

Para jogar com um headset (por exemplo Meta Quest), é preciso um dispositivo compatível com OpenXR. **Sem headset**, dá para testar no editor com o **XR Device Simulator**, que já está na cena.

## Como abrir o projeto

1. Instale o **Unity Hub** e a versão **6000.3.0f1** do editor.
2. Clone o repositório:
   ```bash
   git clone https://github.com/Secondary-Character-A/Top-III.git
   ```
3. No Unity Hub, clique em **Add > Add project from disk** e selecione a pasta clonada.
4. Aguarde a importação. **A primeira abertura demora**, porque a Unity recria a pasta `Library` (ela não vai para o Git).
5. Abra a cena `Assets/PuzzleVazio.unity` (dois cliques) e aperte **Play**.

## Estrutura do projeto

```
Assets/
├── PuzzleVazio.unity     Cena principal do jogo
├── Scripts/              Scripts do puzzle (veja abaixo)
├── Imagens/              Imagens usadas nos puzzles
├── Materiais/            Material base das peças
├── Samples/              Exemplos do XR Interaction Toolkit (Starter Assets, Device Simulator etc.)
├── XRI_Examples/         Apenas o necessário do projeto de exemplos da Unity (XR Origin e controles)
├── XR/ e XRI/            Configurações de XR
└── TextMesh Pro/         Fontes e recursos de texto
Packages/                 Dependências do projeto
ProjectSettings/          Configurações do projeto
```

### Scripts (`Assets/Scripts`)

| Script | Função |
| --- | --- |
| `PuzzleBuilder` | Gera o puzzle: corta a imagem em peças, cria o quadro, a mesa, os encaixes e embaralha as peças. Também reinicia e reconstrói o puzzle. |
| `PuzzleMenu` | Cria o menu em 3D (escolha de tamanho, escolha de imagem, botões Reiniciar e Trocar tamanho). |
| `PuzzleManager` | Confere se todos os encaixes estão corretos e mostra a mensagem e o som de vitória. |
| `PuzzleSlot` | Encaixe de cada posição do quadro. Sabe qual peça deve receber. |
| `PuzzlePiece` | Guarda o identificador de cada peça. |

## Configuração no Inspector

Na cena `PuzzleVazio`, selecione os objetos **PuzzleBuilder** e **PuzzleMenu**:

- **PuzzleBuilder:** `Material Base`, tamanho do quadro (`Tamanho Quadro`), altura da mesa (`Mesa Abaixo`), rotação e espelhamento da imagem nas peças.
- **PuzzleMenu:** lista de `Imagens`, lista de `Tamanhos` (por padrão 2 e 3) e `Max Pecas` (limite de peças, por padrão 10).

Para **adicionar uma imagem nova**, coloque o arquivo em `Assets/Imagens/` e acrescente na lista `Imagens` do **PuzzleMenu**.

## Créditos e licença

- Baseado no projeto **XR Interaction Toolkit Examples (versão 3.4.0)**, da Unity. Veja `LICENSE.md` e `Assets/XRI_Examples/LICENSE.md`.
