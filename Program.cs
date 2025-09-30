using System;
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
                        Console.WriteLine($"{chocolateType.Key}. {chocolateType.Value.Name}");
                    }

                    Console.WriteLine("Zadej ID čokolády k výrobě: ");
                    int ChocolateChoiceID = int.Parse(Console.ReadLine());
                    if (chocolateTypes.ContainsKey(ChocolateChoiceID))
                    {
                        factory.MakeChocolate(ChocolateChoiceID);
                    }
                    else
                    {
                        Console.WriteLine("Neplatny id čokolády.");
                    }
                    break;
                    
                case "2":
                    // Restock ingredients
                    break;
                case "3":
                    // Eat chocolate
                    break;
                case "4":
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
            public Dictionary<int, ChocolateIngredientsInfo> Ingredients { get; set; }
            public Dictionary<int, ChocolateTypeInfo> ChocolateTypes { get; set; }

            public Dictionary<string, int> ProducedChocolates = new Dictionary<string, int>();

            public Chocolate MakeChocolate(int chocolateID)
            {

                //ověření že typ čokošky existuje
                if (!ChocolateTypes.ContainsKey(chocolateID))
                {
                    Console.WriteLine("Neplatný název čokolády.");
                    return null;
                }
                var chocolateType = ChocolateTypes[chocolateID];


                return null;
            }
            private bool CheckIngredients()
            {
                return true; //test
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
                string relativePath = "data\\VyrobnaCokolady.db";
                string filePath = Path.Combine(baseDirectory, relativePath);

                connPath = $"Data Source={filePath}";
            }
            
            public Dictionary<int, ChocolateIngredientsInfo> LoadIngredients()
            {
                
                using (conn = new SQLiteConnection(connPath))
                {
                    conn.Open();

                    
                    SQLiteCommand cmdSelect = conn.CreateCommand();
                    cmdSelect.CommandText = "SELECT * FROM Ingredients;";

                    // vytvori dictionary kde key je ID čokolády a hodnotou je objekt s informacemi o cokolade
                    Dictionary<int, ChocolateIngredientsInfo> _ChocolateIngredient = new Dictionary<int, ChocolateIngredientsInfo>();

                    using (SQLiteDataReader dr = cmdSelect.ExecuteReader())
                    {
                        
                        while (dr.Read())
                        {
                            // id == int
                            int id = Convert.ToInt32(dr["id"]);
                            //----------------------------------------------------------
                            var info = new ChocolateIngredientsInfo // novej objekt s informacema o cokolade
                            {
                                ingredient = dr["ingredient"].ToString(),
                                amount = Convert.ToInt32(dr["amount"].ToString())
                            };
                            // prida novej objekt do dictoinary a ma key = id
                            _ChocolateIngredient[id] = info;
                        }
                        dr.Close();
                    }
                    // vraci dictionary s informacema o cokoladach
                    return _ChocolateIngredient;

                }
            }
            public Dictionary<int, ChocolateTypeInfo> LoadChocolateType()
            {
                
                using (conn = new SQLiteConnection(connPath))
                {
                    conn.Open();

                    
                    SQLiteCommand cmdSelect = conn.CreateCommand();
                    cmdSelect.CommandText = "SELECT * FROM ChocolateType;";

                    // vytvori dictionary kde key je ID čokolády a hodnotou je objekt s informacemi o cokolade
                    Dictionary<int, ChocolateTypeInfo> _ChocolateTypes = new Dictionary<int, ChocolateTypeInfo>();

                    using (SQLiteDataReader dr = cmdSelect.ExecuteReader())
                    {
                        
                        while (dr.Read())
                        {
                            // id == int
                            int id = Convert.ToInt32(dr["id"]);
                            //----------------------------------------------------------
                            var info = new ChocolateTypeInfo // novej objekt s informacema o cokolade
                            {
                                Name = dr["name"].ToString(),
                                CocoaBeans = dr["CocoaBeans"].ToString(),
                                Sugar = dr["Sugar"].ToString(),
                                Milk = dr["Milk"].ToString(),
                                Vanilla = dr["Vanilla"].ToString()
                            };
                            // prida novej objekt do dictoinary a ma key = id
                            _ChocolateTypes[id] = info;
                        }
                        dr.Close();
                    }
                    // vraci dictionary s informacema o cokoladach
                    return _ChocolateTypes;

                }
            }
        }
        public class ChocolateIngredientsInfo //tuhle mrdkotridu naplnuje LoadIngredients()
        {
            
            public string ingredient { get; set; }
            public int amount { get; set; }
        }


        
        public class ChocolateTypeInfo //tuhle mrdkotridu naplnuje LoadChocolateType()
        {

            public string Name { get; set; }
            public string CocoaBeans { get; set; }
            public string Sugar { get; set; }
            public string Milk { get; set; }
            public string Vanilla { get; set; }

        }
        
    }
}

