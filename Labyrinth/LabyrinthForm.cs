using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Labyrinth
{
    public partial class LabyrinthForm : Form
    {
        List<Brick> Walls = new List<Brick>();

        List<Brick> Cells = new List<Brick>();

        //List<Brick> Visited = new List<Brick>();

        Stack<Brick> Visited = new Stack<Brick>();

        Point UpperLeftBorder;
        Point UpperRightBorder;
        Point DownLeftBorder;
        Point DownRightBorder;

        Brick CurrentSelection;
        Brick ExitPosition;
        Brick Player;
        Brick HiddenBrick;
        Brick Finish;


        int BrickWidth = 15;
        int BrickHeight = 15;

        enum Direction
        {
            Up = 1,
            Right,
            Down,
            Left
        }

        public LabyrinthForm()
        {
            InitializeComponent();
        }

        private void MakeCorridors()
        {
            Random rand = new Random();

            Direction direction;

            while(Visited.Count != 0)
            {
                List<Direction> AllPossiblesDiretions = new List<Direction>() { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
                do
                {
                    if (AllPossiblesDiretions.Count != 0)
                    {
                        int randomElement;

                        randomElement = rand.Next(0, AllPossiblesDiretions.Count);

                        direction = AllPossiblesDiretions.ElementAt(randomElement);
                    }
                    else
                    {
                        break;
                    }
                    if (!CanMove(direction))
                    {
                        AllPossiblesDiretions.Remove(direction);
                    }
                    else
                    {
                        BreakWall((Direction)direction);
                        MoveCurrentCell((Direction)direction);
                        AllPossiblesDiretions = new List<Direction>() { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
                    }
                }
                while (/*!CanMove(direction)*/true);

                CurrentSelection.BackColor = System.Drawing.Color.White;
                Visited.Pop();
                if (Visited.Count == 0)
                    break;
                CurrentSelection = Visited.Peek();
                
                CurrentSelection.BackColor = System.Drawing.Color.Green;

            }
        
        }

        private void MoveCurrentCell(Direction direction)
        {
            CurrentSelection.BackColor = System.Drawing.Color.White;
            switch (direction)
            {
                case Direction.Left:
                    CurrentSelection = Cells.Where(x => x.Location.X == (CurrentSelection.Location.X - (CurrentSelection.Width * 2)) && x.Location.Y == CurrentSelection.Location.Y).First();
                    break;
                case Direction.Up:
                    CurrentSelection = Cells.Where(x => x.Location.Y == (CurrentSelection.Location.Y - (CurrentSelection.Height * 2)) && x.Location.X == CurrentSelection.Location.X).First();
                    break;
                case Direction.Down:
                    CurrentSelection = Cells.Where(x => x.Location.Y == (CurrentSelection.Location.Y + (CurrentSelection.Height * 2)) && x.Location.X == CurrentSelection.Location.X).First();
                    break;
                case Direction.Right:
                    CurrentSelection = Cells.Where(x => x.Location.X == (CurrentSelection.Location.X + (CurrentSelection.Width * 2)) && x.Location.Y == CurrentSelection.Location.Y).First();
                    break;
            }

            CurrentSelection.BackColor = System.Drawing.Color.Green;

            CurrentSelection.IsVisited = true;

            Visited.Push(CurrentSelection);

            //this.Refresh();
        }

        private bool CanMove(Direction direction)
        {
            //Проверка на границы и посещяемость
            switch (direction)
            {
                case Direction.Left:
                    if ((CurrentSelection.Location.X - (CurrentSelection.Width * 2)) < UpperLeftBorder.X)//Левый предел
                        return false;
                    var tergetCell = Cells.Where(x => x.Location.X == (CurrentSelection.Location.X - (CurrentSelection.Width * 2)) && x.Location.Y == CurrentSelection.Location.Y).First();
                    if (tergetCell.IsVisited == true)
                        return false;
                    break;
                case Direction.Up:
                    if ((CurrentSelection.Location.Y - (CurrentSelection.Height * 2)) < UpperLeftBorder.Y)//Верхний предел
                        return false;
                    tergetCell = Cells.Where(x => x.Location.Y == (CurrentSelection.Location.Y - (CurrentSelection.Height * 2)) && x.Location.X == CurrentSelection.Location.X).First();
                    if (tergetCell.IsVisited == true)
                        return false;
                    break;
                case Direction.Down:
                    if ((CurrentSelection.Location.Y + (CurrentSelection.Height * 2)) >= DownLeftBorder.Y)//Нижний предел
                        return false;
                    tergetCell = Cells.Where(x => x.Location.Y == (CurrentSelection.Location.Y + (CurrentSelection.Height * 2)) && x.Location.X == CurrentSelection.Location.X).First();
                    if (tergetCell.IsVisited == true)
                        return false;
                        break;
                case Direction.Right:
                    if ((CurrentSelection.Location.X + (CurrentSelection.Width * 2)) >= UpperRightBorder.X)//Правый предел
                        return false;
                    tergetCell = Cells.Where(x => x.Location.X == (CurrentSelection.Location.X + (CurrentSelection.Width * 2)) && x.Location.Y == CurrentSelection.Location.Y).First();
                    if (tergetCell.IsVisited == true)
                        return false;
                    break;
            }
            return true; //Можно двигаться
        }

        private void BreakWall(Direction direction)
        {
            //direction = Direction.Down;
            ////////////////
            //Вычисляем цель
            ////////////////
            int targetX = 0;
            int targetY = 0;
            switch (direction)
            {
                case Direction.Right:
                    targetX = CurrentSelection.Location.X + CurrentSelection.Width;
                    targetY = CurrentSelection.Location.Y;
                    break;
                case Direction.Up:
                    targetX = CurrentSelection.Location.X;
                    targetY = CurrentSelection.Location.Y - CurrentSelection.Height;
                    break;
                case Direction.Down:
                    targetX = CurrentSelection.Location.X;
                    targetY = CurrentSelection.Location.Y + CurrentSelection.Height;
                    break;
                case Direction.Left:
                    targetX = CurrentSelection.Location.X - CurrentSelection.Width;
                    targetY = CurrentSelection.Location.Y;
                    break;
            }
            
  
            //only right
            var target = Walls.Where(anonimusbrick => anonimusbrick.Location.X == targetX && anonimusbrick.Location.Y == targetY).FirstOrDefault();

            if (target == null)
                return;
            ////////////////
            //Вычисляем цель
            ////////////////

            //////////////
            //Ломаем стену
            //////////////
            target.BackColor = System.Drawing.Color.White;
            target.IsVisited = true;

            //this.Controls.Remove(target);

            //CurrentSelection.BackColor = System.Drawing.Color.White;

            Walls.Remove(target);

            Cells.Add(target);

            //////////////
            //Ломаем стену
            //////////////
        }

        private void GenerateLabyrinth(int width, int height)
        {
            int x = 0;

            int y = 0;

            for (int i = 0; i < height; i++)
            {
                
                for (int j = 0; j < width; j++)
                {
                    Brick brick = new Brick(); //Создаем кирпич

                    brick.Location = new System.Drawing.Point(x, y); //Выбираем положение кирпича в пространстве
                    brick.Size = new System.Drawing.Size(BrickWidth, BrickHeight); //Выбираем размер кирпича
                    brick.TabIndex = 1;
                    brick.TabStop = false;

                    if (i == 0 && j == 0)
                    {
                        UpperLeftBorder = new Point(brick.Location.X + brick.Width, brick.Location.Y + brick.Height);
                    }
                    
                    if(i == 0 && j == width - 1)
                    {
                        UpperRightBorder = new Point(brick.Location.X + brick.Width, brick.Location.Y + brick.Height);
                    }

                    if(i == height - 1 && j == 0)
                    {
                        DownLeftBorder = new Point(brick.Location.X + brick.Width, brick.Location.Y + brick.Height);

                    }

                    if (i == height - 1 && j == width - 1)
                    {
                        DownRightBorder = new Point(brick.Location.X + brick.Width, brick.Location.Y + brick.Height);
                    }

                    if ((i % 2 != 0 && j % 2 != 0) && (i < height - 1 && j < width - 1))   //если ячейка нечетная по x и y, и при этом находится в пределах стен лабиринта
                    {
                        brick.BackColor = System.Drawing.Color.White;
                        Cells.Add(brick);
                        brick.Type = "Cell";
                    }
                    else
                    {
                        brick.BackColor = System.Drawing.Color.Black;   //в остальных случаях это ПРОХОД.
                        Walls.Add(brick);
                        brick.Type = "Wall";
                    }
                    x += BrickWidth;
                    this.Controls.Add(brick);  // Добавляю згинерированый кирпич в колекцию контролов нашей формы.
                }
                x = 0;
                y += BrickHeight;
            }
            //CurrentSelection = Cells.First();

            Random r = new Random();

            var number = r.Next(0, Cells.Count-1);

            CurrentSelection = Cells.ElementAt(number);

            CurrentSelection.BackColor = System.Drawing.Color.Green;
            CurrentSelection.IsVisited = true;

            Visited.Push(CurrentSelection);

            
        }

        

      

        private void LabyrinthForm_Load(object sender, EventArgs e)
        {
            
        }

        private void LabyrinthForm_Shown(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(button1);
            this.GenerateLabyrinth(21, 21);
            //this.Refresh();
            this.MakeCorridors();
            CreatePlayer();
            CreateFinish();
            this.Refresh();
        }

        private void CreatePlayer()
        {
            Brick brick = new Brick(); //Создаем кирпич
            brick.Size = new System.Drawing.Size(BrickHeight, BrickWidth); //Выбираем размер кирпича
            brick.TabIndex = 1;
            brick.TabStop = false;
            brick.BackColor = System.Drawing.Color.Blue;

            Random random = new Random();
            var number = random.Next(0, Cells.Count);
            Point point = new Point(Cells.ElementAt(number).Location.X, Cells.ElementAt(number).Location.Y);
            Cells.ElementAt(number).Visible = false;
            HiddenBrick = Cells.ElementAt(number);

            brick.Location = new System.Drawing.Point(point.X, point.Y); //Выбираем положение кирпича в пространстве
            Player = brick;
            this.Controls.Add(brick);
            //this.Refresh();
        }

        private void CreateFinish()
        {
            Brick brick = new Brick(); //Создаем кирпич
            brick.Size = new System.Drawing.Size(BrickHeight, BrickWidth); //Выбираем размер кирпича
            brick.TabIndex = 1;
            brick.TabStop = false;
            brick.BackColor = System.Drawing.Color.Red;

            Random random = new Random();
            var number = random.Next(0, Cells.Count-1);
            Point point = new Point(Cells.ElementAt(number).Location.X, Cells.ElementAt(number).Location.Y);

            var HiddenFinishBrick = Cells.ElementAt(number);

            HiddenFinishBrick.Visible = false;

           

            brick.Location = new System.Drawing.Point(point.X, point.Y); //Выбираем положение кирпича в пространстве

            Finish = brick;

            this.Controls.Add(brick);
            //this.Refresh();
        }

        private void MovePlayer(Direction direction)
        {
            if(!CanMovePlayer(direction))
            {
                return;
            }

            HiddenBrick.Visible = true;
            Point point = new Point();
            Brick target = new Brick();
            switch (direction)
            {
                case Direction.Right:
                    point = new Point(Player.Location.X + BrickWidth, Player.Location.Y);
                    target = Cells.Where(x => x.Location.X == HiddenBrick.Location.X + BrickWidth && x.Location.Y == HiddenBrick.Location.Y).First();
                    break;
                case Direction.Up:
                    point = new Point(Player.Location.X, Player.Location.Y - BrickHeight);
                    target = Cells.Where(x => x.Location.X == HiddenBrick.Location.X && x.Location.Y == HiddenBrick.Location.Y - BrickHeight).First();
                    break;
                case Direction.Down:
                    point = new Point(Player.Location.X, Player.Location.Y + BrickHeight);
                    target = Cells.Where(x => x.Location.X == HiddenBrick.Location.X && x.Location.Y == HiddenBrick.Location.Y + BrickHeight).First();
                    break;
                case Direction.Left:
                    point = new Point(Player.Location.X - BrickWidth, Player.Location.Y);
                    target = Cells.Where(x => x.Location.X == HiddenBrick.Location.X - BrickWidth && x.Location.Y == HiddenBrick.Location.Y).First();
                    break;
            }
            HiddenBrick = target;
            HiddenBrick.Visible = false;

            Player.Location = point;

            if (CheckGameIsOver(direction))
                GameIsOver();


        }

        private bool CanMovePlayer(Direction direction)
        {
            //Проверка на границы и посещяемость
            Brick targetCell = new Brick();
            switch (direction)
            {
                case Direction.Left:
                    if ((Player.Location.X - (Player.Width )) < UpperLeftBorder.X)//Левый предел
                        return false;
                    targetCell = Walls.Where(x => x.Location.X == (Player.Location.X - (Player.Width )) && x.Location.Y == Player.Location.Y).FirstOrDefault();
                    
                    break;
                case Direction.Up:
                    if ((Player.Location.Y - (Player.Height )) < UpperLeftBorder.Y)//Верхний предел
                        return false;
                    targetCell = Walls.Where(x => x.Location.Y == (Player.Location.Y - (Player.Height )) && x.Location.X == Player.Location.X).FirstOrDefault();
                   
                    break;
                case Direction.Down:
                    if ((Player.Location.Y + (Player.Height )) >= DownLeftBorder.Y)//Нижний предел
                        return false;
                    targetCell = Walls.Where(x => x.Location.Y == (Player.Location.Y + (Player.Height )) && x.Location.X == Player.Location.X).FirstOrDefault();
                    
                    break;
                case Direction.Right:
                    if ((Player.Location.X + (Player.Width )) >= UpperRightBorder.X)//Правый предел
                        return false;
                    targetCell = Walls.Where(x => x.Location.X == (Player.Location.X + (Player.Width )) && x.Location.Y == Player.Location.Y).FirstOrDefault();
                    
                    break;
            }
            if (targetCell != null)
                return false;
            return true; //Можно двигаться
        }

        private bool CheckGameIsOver(Direction direction)
        {
            Brick target = new Brick();
            switch (direction)
            {
                case Direction.Right:
                    target = Cells.Where(x => x.Location.X == Finish.Location.X + BrickWidth && x.Location.Y == Finish.Location.Y).First();
                    break;
                case Direction.Up:
                    target = Cells.Where(x => x.Location.X == Finish.Location.X && x.Location.Y == Finish.Location.Y - BrickHeight).First();
                    break;
                case Direction.Down:
                    target = Cells.Where(x => x.Location.X == Finish.Location.X && x.Location.Y == Finish.Location.Y + BrickHeight).First();
                    break;
                case Direction.Left:
                    target = Cells.Where(x => x.Location.X == Finish.Location.X - BrickWidth && x.Location.Y == Finish.Location.Y).First();
                    break;
            }
            if (target.Location.X == Finish.Location.X && target.Location.Y == Finish.Location.Y)
                return true;
            else
                return false;
        }

        private void GameIsOver()
        {
            foreach (var item in this.Controls)
            {
                Controls.Remove((System.Windows.Forms.Control)item);
            }
        }

        private void LabyrinthForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case (Keys.Up):
                    MovePlayer(Direction.Up);
                    break;
                case (Keys.Right):
                    MovePlayer(Direction.Right);
                    break;
                case (Keys.Down):
                    MovePlayer(Direction.Down);
                    break;
                case (Keys.Left):
                    MovePlayer(Direction.Left);
                    break;
                default:
                    break;
            }
            //this.Refresh();
        }
    }
}