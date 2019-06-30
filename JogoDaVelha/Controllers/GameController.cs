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
        //// GET: api/Game
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Game/5
        //public string Get(int id)
        //{
        //    Game myGame = new Game();
        //    //myGame.map[0] = myGame.row0;
        //    //myGame.map[1] = myGame.row1;
        //    //myGame.map[2] = myGame.row2;

        //    //myGame.map[2][2] = "X";

        //    Game myGame2 = new Game();
        //    //return "value";
        //    //return myGame.map[2][2];
        //    string lala = "mg1 = " + myGame.id.ToString() + "; mg2 = " + myGame2.id.ToString();
        //    return lala;
        //}

        // POST: /game
        [Route("game")]
        public IHttpActionResult Post()
        {
            Game myGame = new Game();
            string[] emptyRow = new string[3];
            myGame.map[0] = emptyRow;
            myGame.map[1] = emptyRow;
            myGame.map[2] = emptyRow;

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

            myGame.nextPlayer = myGame.firstPlayer;

            XmlSerializer xsMyGame = new XmlSerializer(typeof(Game));
            string path = @"c:\temp\dti\" + myGame.id + ".xml";
            TextWriter tw = new StreamWriter(path);
            xsMyGame.Serialize(tw, myGame);
            tw.Close();

            return Ok(new { myGame.id, myGame.firstPlayer });
        }

        // POST: /game/{id}/movement
        [Route("game/{id}/movement")]
        public IHttpActionResult Post([FromBody]dynamic value)
        {
            string id = value.id;
            string player = value.player.ToString().ToUpper();
            int positionX = value.position.x;
            int positionY = value.position.y;

            if (player != "X" && player != "O")
            {
                return Ok(new { msg = "Jogada inválida; jogue um X ou O" });
            }

            string path = @"c:\temp\dti\" + id + ".xml";
            if (!File.Exists(path))
            {
                return Ok(new { msg = "Partida não encontrada" });
            }

            var sr = new StreamReader(path);
            XmlSerializer xsMyGame = new XmlSerializer(typeof(Game));
            Game myGame = (Game)xsMyGame.Deserialize(sr);
            sr.Close();

            var message = CheckBeforeMovement(myGame);

            if (IsFinished(myGame))
            {
                return Ok(new { status = "Partida finalizada", winner = myGame.winner });
            }

            if (myGame.nextPlayer != player)
            {
                return Ok(new { msg = "Não é turno do jogador" });
            }

            if (myGame.map[positionX][positionY] != null)
            {
                return Ok(new { msg = string.Format("Posição [{0},{1}] já foi marcada; jogue em outra posição!", positionX, positionY) });
            }



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

            myGame.winner = CheckForWinner(myGame);

            TextWriter tw = new StreamWriter(path);
            xsMyGame.Serialize(tw, myGame);
            tw.Close();

            if (IsFinished(myGame))
            {
                return Ok(new { status = "Partida finalizada", winner = myGame.winner });
            }

            return Ok(new { code = 200 });
        }

        private string CheckBeforeMovement(Game myGame)
        {
            var message = string.Empty;

            return message;
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

        //// POST: api/Game
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Game/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Game/5
        //public void Delete(int id)
        //{
        //}
    }
}
