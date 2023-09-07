using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

public class Board
{
    public char[] state;
    public List<int> availablePositions;

    public Board() {
        this.state = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        this.availablePositions = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }
    public Board(char[]st,List<int>ap) {
        this.state = st;
        this.availablePositions = ap;
    }

    public void applyMove(int move,char player)
    {
        state[move - 1] = player;
        availablePositions.Remove(move);
    }

    public int checkGameEnded() {
        if ((state[0] == 'X' && state[1] == 'X' && state[2] == 'X') ||
            (state[3] == 'X' && state[4] == 'X' && state[5] == 'X') ||
            (state[6] == 'X' && state[7] == 'X' && state[8] == 'X') ||
            (state[0] == 'X' && state[3] == 'X' && state[6] == 'X') ||
            (state[1] == 'X' && state[4] == 'X' && state[7] == 'X') ||
            (state[2] == 'X' && state[5] == 'X' && state[8] == 'X') ||
            (state[0] == 'X' && state[4] == 'X' && state[8] == 'X') ||
            (state[2] == 'X' && state[4] == 'X' && state[6] == 'X'))
        {
            return 1;
        }
        else if ((state[0] == 'O' && state[1] == 'O' && state[2] == 'O') ||
            (state[3] == 'O' && state[4] == 'O' && state[5] == 'O') ||
            (state[6] == 'O' && state[7] == 'O' && state[8] == 'O') ||
            (state[0] == 'O' && state[3] == 'O' && state[6] == 'O') ||
            (state[1] == 'O' && state[4] == 'O' && state[7] == 'O') ||
            (state[2] == 'O' && state[5] == 'O' && state[8] == 'O') ||
            (state[0] == 'O' && state[4] == 'O' && state[8] == 'O') ||
            (state[2] == 'O' && state[4] == 'O' && state[6] == 'O'))
        {
            return 2;
        }
        else if (availablePositions.Count==0)
        {
            return 0;
        }
        else {
            return -1;
        }
    }

    public void print() {
        Console.WriteLine($" {state[0]} | {state[1]} | {state[2]} ");
        Console.WriteLine("---+---+---");
        Console.WriteLine($" {state[3]} | {state[4]} | {state[5]} ");
        Console.WriteLine("---+---+---");
        Console.WriteLine($" {state[6]} | {state[7]} | {state[8]} ");
    }

    public Board Clone() {
        return new Board((char[])this.state.Clone(), new List<int>(this.availablePositions));
    }
}
class Program
{
    static Board board = new Board();
    static bool gameEnded = false;
    static string endingStatement = "";

    static void Main()
    {
        Console.WriteLine("Welcome to Tic Tac Toe!");
        //Draw the board
        DrawBoard();
        do
        {
            //Player's move
            Console.Write("\nPlayer's turn (X): ");
            int playerChoice;
            if (int.TryParse(Console.ReadLine(), out playerChoice))
            {
                if (playerChoice >= 1 && playerChoice <= 9 && IsValidMove(playerChoice))
                {
                    board.applyMove(playerChoice, 'X');
                }
                else
                {
                    Console.WriteLine("Invalid move! Press any key to retry...");
                    Console.ReadKey();
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Invalid input! Press any key to retry...");
                Console.ReadKey();
                continue;
            }

            //Check if player won
            CheckGameEnded();
            DrawBoard();

            if (!gameEnded) {
                //AI's move
                int aiMove = AiMove(board, 'O', 1);
                board.applyMove(aiMove,'O');
            }

            //Check if player won
            CheckGameEnded();
            DrawBoard();

        } while (!gameEnded);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static bool IsValidMove(int choice)
    {
        if (!board.availablePositions.Contains(choice))
        {
            return false;
        }
        else {
            return true;
        }
    }


    static void CheckGameEnded() {
        switch (board.checkGameEnded()) {
            case 1:
                gameEnded = true;
                endingStatement = "Player 1(X) has won!";
                break;
            case 2:
                gameEnded = true;
                endingStatement = "Player 2(O) has won!";
                break;
            case 0:
                gameEnded = true;
                endingStatement = "It's a draw!";
                break;
            default:
                break;
        }
    }

    static void DrawBoard()
    {
        Console.Clear();
        Console.WriteLine("Player 1: X and Player 2: O");
        Console.WriteLine("\n");
        board.print();
        Console.WriteLine(endingStatement);
    }

    static int AiMove(Board currentBoard, char currentPlayer, int depth)
    {
        var clonedBoard = currentBoard.Clone();
        var availableMoves = clonedBoard.availablePositions;

        var topScore = int.MinValue;
        var topMove = -1;

        foreach (var move in availableMoves)
        {
            var score = MiniMax(currentPlayer,move,clonedBoard.Clone(),1);

            if (score > topScore)
            {
                topScore = score;
                topMove = move;
            }
        }

        return topMove;
    }



    static int MiniMax(char currentPlayer,int move,Board board,int depth)
    {
        board.applyMove(move,currentPlayer);
        char opponent = (currentPlayer == 'O') ? 'X' : 'O';

        //Check if reached terminal node
        switch (board.checkGameEnded())
        {
            case 1:
                //Big punishment for losing
                return -100;
            case 2:
                //Huge reward for winning
                return 100;
            case 0:
                //Small punishment for draw
                return -10;
            default:
                break;
        }

        //Game continues
        depth++;
        var nextAvailableMoves = board.availablePositions;
        int maxScore = int.MinValue;
        int minScore = int.MaxValue;

        for (var i = 0; i < nextAvailableMoves.Count; i++)
        {
            var score = MiniMax(opponent, nextAvailableMoves[i], board.Clone(), depth);
            if (score > maxScore)
            {
                maxScore = score;
            }
            if (score < minScore)
            {
                minScore = score;
            }
        }
        return currentPlayer == 'O' ? minScore : maxScore;
    }

}
