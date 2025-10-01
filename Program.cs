using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KaplanChocolateFactory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            DatabaseManager dbManager = new DatabaseManager();
            ChocolateFactory factory = new ChocolateFactory();
            factory.Ingredients = dbManager.LoadIngredients();
            var chocolateTypes = dbManager.LoadChocolateType();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Welcome to Kaplan's Chocolate Factory!");
            sb.AppendLine("Co bys chtěl udělat?");
            sb.AppendLine("1. Vyrobit čokoládu");
            sb.AppendLine("2. Doplnit suroviny");
            sb.AppendLine("3. Sníst čokoládu");
            sb.AppendLine("4. Ukaž vyrobené čokolády");
            sb.AppendLine("5. Ukončit program");
            Console.WriteLine(sb.ToString());
            switch(Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Volitelné čokolády:");
                    foreach(var chocolateType in chocolateTypes)
                    {
                        Console.WriteLine($"{chocolateType.Key} ");
                    }
                    while (true)
                    {
                        Console.WriteLine("Zadej jméno čokolády k výrobě: ");
                        string ChocolateChoiceID = Console.ReadLine();
                        if (chocolateTypes.ContainsKey(ChocolateChoiceID))
                        {
                            factory.MakeChocolate(ChocolateChoiceID);
                            
                        }
                        else
                        {
                            Console.WriteLine("Neplatny jméno čokolády.");
                        }
                    }
                    
                    
                    
                case "2":
                    // Restock ingredients
                    break;
                case "3":
                    // Eat chocolate
                    break;
                case "4":
                    //Ukaž vyrobené čokolády
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Neplatná volba. Zkus to znovu.");
                    break;
            }

            

            



        }
        //public string EatChocolate(Chocolate chocolate)
        //{
        //    return $"You have eaten the {chocolate.name} chocolate!";
        //}

        class ChocolateFactory
        {
            public Dictionary<string, ChocolateIngredientsInfo> Ingredients { get; set; }
            public Dictionary<string, ChocolateTypeInfo> ChocolateTypes { get; set; }

            public Dictionary<string, int> ProducedChocolates = new Dictionary<string, int>();

            public ChocolateTypeInfo MakeChocolate(string chocolateNAME)
            {

                //ověření že typ čokošky existuje
                if (!ChocolateTypes.ContainsKey(chocolateNAME))
                {
                    Console.WriteLine("Neplatný název čokolády.");
                    return null;
                }
                else
                {
                    ChocolateTypeInfo chocolateType = ChocolateTypes[chocolateNAME];

                    CheckIngredients(chocolateType);

                }

                    



                return null;
            }
            private bool CheckIngredients(ChocolateTypeInfo ing)
            {

                // Zkontrolujeme CocoaBeans
                if (!Ingredients.ContainsKey(ing.CocoaBeans.Name) || Ingredients["CocoaBeans"].amount < ing.CocoaBeans)
                    return false;

                // Zkontrolujeme Sugar
                if (!Ingredients.ContainsKey("Sugar") || Ingredients["Sugar"].amount < ing.Sugar)
                    return false;

                // Zkontrolujeme Milk
                if (!Ingredients.ContainsKey("Milk") || Ingredients["Milk"].amount < ing.Milk)
                    return false;

                // Zkontrolujeme Vanilla
                if (!Ingredients.ContainsKey("Vanilla") || Ingredients["Vanilla"].amount < ing.Vanilla)
                    return false;

                return true; // všechny podmínky splněné, můžeme vyrobit
               
            }

            //public int RestockIngredient(string IngredientName, int Amount)
            //{

            //}
        }
        class Chocolate
        {
            string name { get; set; }
            int producedAmount { get; set; }

            public Chocolate(string name)
            {
                this.name = name;
                this.producedAmount = producedAmount;
            }
        }

        class DatabaseManager
        {

            //atribut connection
            SQLiteConnection conn;

            string connPath;
            public DatabaseManager()
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = "data\\chocolateDB.db";
                string filePath = Path.Combine(baseDirectory, relativePath);

                connPath = $"Data Source={filePath}";
            }

            //INGREDIENCE LOUDOVANI
            public Dictionary<string, ChocolateIngredientsInfo> LoadIngredients()
            {
                using (conn = new SQLiteConnection(connPath))
                {
                    conn.Open();

                    SQLiteCommand cmdSelect = conn.CreateCommand();
                    cmdSelect.CommandText = "SELECT * FROM Ingredients;";

                    // key už neni int ale string === jméno ingredience
                    Dictionary<string, ChocolateIngredientsInfo> _ChocolateIngredient = new Dictionary<string, ChocolateIngredientsInfo>();

                    using (SQLiteDataReader dr = cmdSelect.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var info = new ChocolateIngredientsInfo
                            {
                                amount = Convert.ToInt32(dr["amount"].ToString())
                            };

                            // klíč je rovnou ingredient
                            _ChocolateIngredient[dr["ingredient"].ToString()] = info;
                        }
                        dr.Close();
                    }
                    return _ChocolateIngredient;
                }
            }

            // LOUDOVANI COKOLADOVYHO TYPU
            public Dictionary<string, ChocolateTypeInfo> LoadChocolateType()
            {
                using (conn = new SQLiteConnection(connPath))
                {
                    conn.Open();

                    SQLiteCommand cmdSelect = conn.CreateCommand();
                    cmdSelect.CommandText = "SELECT * FROM ChocolateType;";

                    Dictionary<string, ChocolateTypeInfo> _ChocolateTypes = new Dictionary<string, ChocolateTypeInfo>();

                    using (SQLiteDataReader dr = cmdSelect.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var info = new ChocolateTypeInfo
                            {
                                CocoaBeans = Convert.ToInt32(dr["CocoaBeans"]),
                                Sugar = Convert.ToInt32(dr["Sugar"]),
                                Milk = Convert.ToInt32(dr["Milk"]),
                                Vanilla = Convert.ToInt32(dr["Vanilla"])
                            };

                            // klíčem je rovnou jméno čokolády
                            _ChocolateTypes[dr["name"].ToString()] = info;
                        }
                        dr.Close();
                    }
                    return _ChocolateTypes;
                }
            }
        }


        public class ChocolateIngredientsInfo //tuhle mrdkotridu naplnuje LoadIngredients()
        {
            public int amount { get; set; }
        }


        
        public class ChocolateTypeInfo //tuhle mrdkotridu naplnuje LoadChocolateType()
        {
            public int CocoaBeans { get; set; }
            public int Sugar { get; set; }
            public int Milk { get; set; }
            public int Vanilla { get; set; }

        }
        
    }
}

