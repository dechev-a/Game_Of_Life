using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    public class Grid
    {
        public enum Flag { ALL, LEFT, RIGHT };
        private bool[,] map;
        private bool[,] tmap;
        private bool gameSetUp;
        private int time;
        private int aliveCellAround;
        private int nbCell;

        //Lance le jeu
        public void startGame ()
        {
            if (gameSetUp == true)
            {
                while (this.nbCell > 2)
                {
                    Console.Clear();
                    this.Display();
                    System.Console.ReadLine();
                    Reaper();
                }
                Console.Clear();
                Console.Write("Seulement ");
                Console.Write(this.nbCell);
                Console.WriteLine(" restait en vie.");
                Console.WriteLine("Game Over");
                Console.ReadLine();
            }
            else
                Console.WriteLine("Please set up the game before trying to start it");
        }
        //constructeur initialisant les différentes variables
        public Grid ()
        {
            map = new bool[50, 50];
            tmap = new bool[50, 50];
            aliveCellAround = 0;
            gameSetUp = false;
        }

        //fonction mettant en place la partie. Elle demande à l'utilisateur le nombre de cellule à placer et les disposent aléatoirement. Ensuite elle propose de laisser l'utilisateurs
        //définir un intervalle de temps entre chaque tour (la gestion automatique des tours n'existe pas encore) (par defaut 100ms)
        public void setUpGame ()
        {
            string snb;
            Random rand;

            rand = new Random();
            Console.WriteLine("Inscrivez le nombre de cellule desiree pour demarrer le jeu:");
            snb = Console.ReadLine();
            nbCell = int.Parse(snb);

            for (int i = 0; i < nbCell; i++ )
                map[(rand.Next(50)), (rand.Next(50))] = true;
            Console.WriteLine("Inscrivez l'interval desiree ou laisse vide pour 100 ms");
            snb = Console.ReadLine();
            if (snb != "")
                time = int.Parse(snb);
            else
                time = 100;

            gameSetUp = true;
        }
        //Affiche la map, o étant une cellule en vie, . une case vide
        private void Display ()
        {
            int size;

            size = 0;
            foreach (bool area in map)
            {
                size++;
                if (area == true)
                    Console.Write('o');
                else
                    Console.Write('.');
                if (size == 50)
                {
                    Console.Write('\n');
                    size = 0;
                }
            }
        }
        //fonction censé verrifier si une cellule doit vivre ou mourir
        private void Reaper ()
        {
            int x;
            int y;

            x = 0;
            y = 0;
            this.nbCell = 0;
            while (x < 50)
            {
                y = 0;
                while (y < 50)
                {
                    if (x == 0 || y == 0 || x == 49 || y == 49)
                        this.checkException(x,y);
                    else
                        this.check(x,y);
                    if (this.aliveCellAround == 3)
                    {
                        this.tmap[x, y] = true;
                        this.nbCell++;
                    }
                    else if (this.aliveCellAround == 2 && this.map[x, y] == true)
                    {
                        this.tmap[x, y] = true;
                        this.nbCell++;
                    }
                    this.aliveCellAround = 0;
                    y++;
                }
                x++;
            }
            this.map = this.tmap;
            this.tmap = new bool[50, 50];
        }

        private void checkException (int x, int y)
        {
            if (x == y || (x == 0 && y == 49) || (x == 49 && y == 0))
                this.checkCorner(x, y);
            else if (x == 0 || x == 49 || y == 0 || y == 49)
                this.checkSide(x, y);
        }
        //Fonction verrifiant les quatres coins de la map
        private void checkCorner (int x, int y)
        {
            if (x == y)
            {
                if (x == 0)
                {
                    this.checkBottom(x, y, 2);
                    this.checkRight(x, y, 1);
                }
                else
                {
                    this.checkTop(x, y, 2);
                    this.checkLeft(x, y, 1);
                }
            }
            else if (x == 49)
            {
                this.checkTop(x, y, 1);
                this.checkRight(x, y, 1);
            }
            else
            {
                this.checkBottom(x, y, 1);
                this.checkLeft(x, y, 1);
            }
        }
       //Fonction verrifiant les côtés de la map
        public void checkSide(int x, int y)
        {
            if (x == 0)
            {
                this.checkBottom(x, y, 0);
                this.checkRight(x, y, 1);
                this.checkLeft(x, y, 1);
            }
            else if (y == 0)
            {
                this.checkTop(x, y, 1);
                this.checkRight(x, y, 1);
                this.checkBottom(x, y, 2);
            }
            else if (x == 49)
            {
                this.checkTop(x, y, 0);
                this.checkRight(x, y, 1);
                this.checkLeft(x, y, 1);
            }
            else
            {
                this.checkTop(x, y, 2);
                this.checkLeft(x, y, 1);
                this.checkBottom(x, y, 1);
            }
        }
        //Les quatre fonctions suivantes vérifient un coté d'une cellule donnée, avec des flags permettant de moduler cette verrification
        private void checkTop (int x, int y, int flag)
        {
            if (flag == 0 || flag == 2)
            {
                if (this.map[x - 1, y - 1] == true) // top left
                    this.aliveCellAround++;
            }
            if (this.map[x - 1, y] == true) // top
                this.aliveCellAround++;
            if (flag == 1 || flag == 0)
            {
                if (this.map[x - 1, y + 1] == true) // top right
                    this.aliveCellAround++;
            }
        }

        private void checkBottom (int x, int y, int flag)
        {
            if (flag == 0 || flag == 2)
            {
                if (this.map[x + 1, y + 1] == true) //right bot
                    this.aliveCellAround++;
            }
            if (this.map[x + 1, y] == true) // bot
                this.aliveCellAround++;
            if (flag == 0 || flag == 1)
            {
                if (this.map[x + 1, y - 1] == true) // bot left
                    this.aliveCellAround++;
            }
         }

        private void checkLeft(int x, int y, int flag)
        {
            if (this.map[x, y - 1] == true) // left
                this.aliveCellAround++;
            if (flag == (int)Flag.ALL)
            {
                if (this.map[x + 1, y - 1] == true) // bot left
                    this.aliveCellAround++;
                if (this.map[x - 1, y - 1] == true) // top left
                    this.aliveCellAround++;
            }
        }

        private void checkRight(int x, int y, int flag)
        {
            if (this.map[x, y + 1] == true) // right
                this.aliveCellAround++;
            if (flag == (int)Flag.ALL)
            {
                if (this.map[x - 1, y + 1] == true) // top right  
                    this.aliveCellAround++;
                if (this.map[x + 1, y + 1] == true) //right bot
                    this.aliveCellAround++;
            }
        }
        //Fonction de vérification générique, utilisable tant que la cellule n'est ni dans un coin ni sur un bord
        private void check (int x, int y)
        {
            checkRight(x, y, 0);
            checkLeft(x, y, 0);
            checkTop(x, y, 3);
            checkBottom(x, y, 3);
        }
    }
}
