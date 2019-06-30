# JogoDaVelha

Esta aplicação consiste numa api para um jogo multiplayer de Jogo da Velha.

Trata-se de uma ASP.NET Web Application - Web API, em linguagem C#.
A aplicação foi desenvolvida no MS Visual Studio 2017, que deve ser utilizado para build.

OBS: não adicionei à solução nada referente a build automatizado.
Entendo que poderia utilizar uma ferramenta como Jenkins, integrada ao Git, para disparar o build automaticamente sempre que for detectada uma mudança no repositório.
Em relação a build automatizado, não entendi o que era esperado como entrega na solução da API propriamente dita.


A chamada POST /game cria um novo jogo, com um ID único, e define um jogador que deverá iniciar a partida: X ou O.
Cada jogo é salvo num arquivo XML, que possui como nome o ID do jogo.

Numa situação real, o diretório onde ficam salvos os arquivos poderia ser armazenado no arquivo web.config, por exemplo.
Por conveniência e por se tratar de uma tarefa curta, nesta aplicação o diretório está hard-coded: C:\Temp\dti

Assim, um exemplo de caminho de arquivo correspondente a um jogo é:
C:\Temp\dti\cb230a95-15d8-4d83-8362-f7d6104cd37c.xml

As jogadas ocorrem em turnos. A cada jogada, o arquivo XML é atualizado.
A chamada /game/{id}/movement executa uma jogada.
A entrada deve conter o ID do jogo, o jogador (X ou O), e a posição (x,y) onde será feita a jogada.

O usuário é notificado nas seguintes situações:
- jogada inválida (diferente de X ou O)
- partida não encontrada (não existe nenhuma partida com o ID informado)
- partida finalizada (usuário tentou jogar numa partida já encerrada)
- turno incorreto (usuário tentou jogar no turno do outro jogador)
- posição já marcada.

Por conveniência, as mensagens estão hard-coded e em português.
Numa situação real, poderiam estar armazenadas em arquivos de localização (ex: strings.resx) para permitir uso em outros idiomas, ou mesmo hard-coded porém atribuídas a variáveis, para manter o código mais limpo.

Quando um jogador vence, ou quando se completam 9 jogadas (o que corresponde ao tabuleiro completo) sem vencedor, a partida é encerrada.


Tarcisio Cortes

2019