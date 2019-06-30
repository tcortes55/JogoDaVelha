using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Serialization;
using JogoDaVelha.Models;
using static System.Net.Mime.MediaTypeNames;

namespace JogoDaVelha.Controllers
{
    public class GameController : ApiController
    {
        // POST: /game
        [Route("game")]
        public IHttpActionResult Post()
        {
            Game myGame = new Game();
            string[] emptyRow = new string[3];
            myGame.map[0] = emptyRow;
            myGame.map[1] = emptyRow;
            myGame.map[2] = emptyRow;

            // Determinamos aleatoriamente se X ou O irá começar o jogo
            Random random = new Random();
            int randomNumber = random.Next();
            if (randomNumber % 2 == 0)
            {
                myGame.firstPlayer = "X";
            }
            else
            {
                myGame.firstPlayer = "O";
            }

            // A primeira jogada será feita pelo jogador sorteado
            myGame.nextPlayer = myGame.firstPlayer;

            // O jogo é salvo num arquivo XML. O nome do arquivo corresponde ao ID do jogo
            XmlSerializer xsMyGame = new XmlSerializer(typeof(Game));

            // O diretório está hard-coded por conveniência por ser uma tarefa curta.
            // Numa situação real, poderia ser armazenado no arquivo web.config, por exemplo.
            string path = @"c:\temp\dti\" + myGame.id + ".xml";

            // Se o diretório não existe, vamos criá-lo...
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            file.Directory.Create();

            TextWriter tw = new StreamWriter(path);
            xsMyGame.Serialize(tw, myGame);
            tw.Close();

            return Ok(new { myGame.id, myGame.firstPlayer });
        }

        // POST: /game/{id}/movement
        [Route("game/{id}/movement")]
        public IHttpActionResult Post([FromBody]dynamic value)
        {
            // Obtemos os valores da request...
            string id = value.id;
            string player = value.player.ToString().ToUpper();
            int positionX = value.position.x;
            int positionY = value.position.y;

            // Somente os valores "X" e "O" são aceitos para a jogada
            if (player != "X" && player != "O")
            {
                // As mensagens ao usuário estão hard-coded por conveniência por ser uma tarefa curta.
                // Numa situação real, poderiam estar armazenadas em arquivos de configuração, banco de dados,
                // ou mesmo hard-coded porém atribuídas a variáveis.
                return Ok(new { msg = "Jogada inválida; jogue um X ou O" });
            }

            // Se o arquivo <ID>.xml não for encontrado, a partida não existe
            string path = @"c:\temp\dti\" + id + ".xml";
            if (!File.Exists(path))
            {
                return Ok(new { msg = "Partida não encontrada" });
            }
            
            Game myGame = GetGameFromFile(path);

            // Antes de executar a jogada, fazemos algumas verificações
            if (IsFinished(myGame))
            {
                return Ok(new { status = "Partida finalizada", myGame.winner });
            }
            if (myGame.nextPlayer != player)
            {
                return Ok(new { msg = "Não é turno do jogador" });
            }
            if (myGame.map[positionX][positionY] != null)
            {
                return Ok(new { msg = string.Format("Posição [{0},{1}] já foi marcada; jogue em outra posição!", positionX, positionY) });
            }

            // Jogada será executada...
            myGame.map[positionX][positionY] = player;
            switch (player)
            {
                case "X":
                    myGame.nextPlayer = "O";
                    break;
                case "O":
                    myGame.nextPlayer = "X";
                    break;
            }

            myGame.numRounds++;

            // Após cada jogada, verificamos se há um vencedor
            myGame.winner = CheckForWinner(myGame);

            SaveGameToFile(myGame, path);

            if (IsFinished(myGame))
            {
                return Ok(new { status = "Partida finalizada", myGame.winner });
            }
            else
            {
                return Ok(new { code = 200 });
            }
        }

        private Game GetGameFromFile(string path)
        {
            var sr = new StreamReader(path);
            XmlSerializer xsMyGame = new XmlSerializer(typeof(Game));
            Game myGame = (Game)xsMyGame.Deserialize(sr);
            sr.Close();

            return myGame;
        }

        void SaveGameToFile(Game myGame, string path)
        {
            TextWriter tw = new StreamWriter(path);
            XmlSerializer xsMyGame = new XmlSerializer(typeof(Game));
            xsMyGame.Serialize(tw, myGame);
            tw.Close();
        }

        private bool IsFinished(Game game)
        {
            if (!String.IsNullOrEmpty(game.winner))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string CheckForWinner(Game game)
        {
            if (game.numRounds < 5)
            {
                // Impossível que o jogo tenha acabado com menos de 5 rodadas
                return string.Empty;
            }

            var winner = string.Empty;

            if (game.map[0][0] == game.map[0][1] && game.map[0][0] == game.map[0][2]) //1a coluna
            {
                winner = game.map[0][0];
            }
            else if (game.map[1][0] == game.map[1][1] && game.map[1][0] == game.map[1][2]) //2a coluna
            {
                winner = game.map[1][0];
            }
            else if (game.map[2][0] == game.map[2][1] && game.map[2][0] == game.map[2][2]) //3a coluna
            {
                winner = game.map[2][0];
            }
            else if (game.map[0][0] == game.map[1][0] && game.map[0][0] == game.map[2][0]) //1a linha
            {
                winner = game.map[0][0];
            }
            else if (game.map[0][1] == game.map[1][1] && game.map[0][1] == game.map[2][1]) //2a linha
            {
                winner = game.map[0][1];
            }
            else if (game.map[0][2] == game.map[1][2] && game.map[0][1] == game.map[2][2]) //3a linha
            {
                winner = game.map[0][2];
            }
            else if (game.map[0][0] == game.map[1][1] && game.map[0][0] == game.map[2][2]) //1a diagonal
            {
                winner = game.map[0][0];
            }
            else if (game.map[0][2] == game.map[1][1] && game.map[0][2] == game.map[2][0]) //2a diagonal
            {
                winner = game.map[0][2];
            }

            //Se nenhum vencedor foi identificado e já se passaram 9 rodadas, houve um empate
            if (String.IsNullOrEmpty(winner) && game.numRounds == 9)
            {
                winner = "Draw";
            }

            return winner;
        }
    }
}
