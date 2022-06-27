using System.Text.Json;
using System.Globalization;

namespace PapersPlease_1
{
    class ObjectSerializer
    {
        static void Main(string[] args)
        {
            List<Passport> acceptedPassports = new List<Passport> ();
            List<Passport> deniedPassports = new List<Passport> ();
            List<Passport> suspectPassports = new List<Passport>();
            int total = 0;
            Console.WriteLine("Введите запрещенный город: ");
            string tabuCity = Console.ReadLine();
            int flag = 1;
            int errors = 0;
            while (flag == 1)
            {
                Console.WriteLine("1 - Ввести данные, 2 - Проверить данные");
                flag = Convert.ToInt32(Console.ReadLine());
                if (flag == 1)
                {
                    Console.WriteLine("Введите паспортные данные:");
                    try
                    {
                        Passport? restoredPerson = JsonSerializer.Deserialize<Passport>(Console.ReadLine().Replace("=", ":"));
                        string[] name = { restoredPerson.fname, restoredPerson.mname, restoredPerson.lname };
                        DateTime bd = DateTime.Parse(restoredPerson.birth.Replace(":", "/"));
                        string city = restoredPerson.city;
                        restoredPerson.errorsStrings = new List<string> ();

                        #region Ввод и форматирование
                        Console.WriteLine("Введите полученные данные: ");
                        //Возраст
                        string[] strings = Console.ReadLine().Split(", ");
                        strings[0] = strings[0].Trim(',');
                        int age = int.Parse(strings[0]); //Не стал делать проверку на int, доверимся входящим данным
                        //ФИО
                        strings[1] = strings[1].Trim('\"');
                        string[] fml = strings[1].Split(" ");
                        if (fml.Length == 3)
                        {
                            string temp = fml[0];
                            fml[0] = fml[1];
                            fml[1] = fml[2];
                            fml[2] = temp;
                        }
                        #endregion

                        #region Проверка возраста
                        int span = Convert.ToInt32(Math.Truncate(DateTime.Now.Subtract(bd).TotalDays / 365));
                        #endregion

                        #region Проверка ФИО
                        for (int i = 0; i < fml.Length; i++)
                        {
                            int a = 0;
                            if (name[i].Length < fml[i].Length) a = name[i].Length;
                            else a = fml[i].Length;
                            errors += Math.Abs(name[i].Length - fml[i].Length);
                            for (int j = 0; j < a; j++)
                            {
                                if (name[i][j] != fml[i][j])
                                {
                                    errors++;
                                }
                            }
                        }
                        #endregion
                        #region Проверка на доступ
                        if ((errors <= 2) && (span == age) && (tabuCity != city))
                        {
                            acceptedPassports.Add(restoredPerson);
                        }
                        else
                        {
                            if (span != age)
                            {
                                restoredPerson.errorsStrings.Add("Неправильный возраст");
                            }
                            if (errors > 2)
                            {
                                restoredPerson.errorsStrings.Add("Неверное ФИО");
                            }
                            if (tabuCity == city)
                            {
                                restoredPerson.errorsStrings.Add("Запрещенный город");
                            }
                            Console.WriteLine("Количество ошибок: " + restoredPerson.errorsStrings.Count);
                            #region Случайно генерируем взятку
                            Random rnd = new Random();
                            if (rnd.Next(2) == 1)
                            {
                                Console.WriteLine("Желаете оплатить? y/n");
                                string ans = Console.ReadLine();
                                if (ans == "y")
                                {
                                    total += restoredPerson.errorsStrings.Count * 250;
                                    suspectPassports.Add(restoredPerson);
                                }
                                else
                                {
                                    deniedPassports.Add(restoredPerson);
                                }
                            }
                            #endregion
                            else
                            {
                                deniedPassports.Add(restoredPerson);
                            }
                        }
                        #endregion
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                #region Вывод
                else
                {
                    Console.WriteLine("Разрешить доступ");
                    for (int i = 0; i < acceptedPassports.Count; i++)
                    {
                        Console.WriteLine(acceptedPassports[i].birth + ' ' + acceptedPassports[i].fname + ' ' + acceptedPassports[i].mname + ' ' + acceptedPassports[i].lname);
                    }
                    Console.WriteLine("Запретить доступ");
                    for (int i = 0; i < deniedPassports.Count; i++)
                    {
                        Console.WriteLine(deniedPassports[i].birth + ' ' + deniedPassports[i].fname + ' ' + deniedPassports[i].mname + ' ' + deniedPassports[i].lname);
                        Console.WriteLine("По причинам: ");
                        for (int j = 0; j < deniedPassports[i].errorsStrings.Count; j++)
                        {
                            Console.WriteLine(deniedPassports[i].errorsStrings[j]);
                        }
                    }
                    Console.WriteLine("Подозрительные паспорта");
                    for (int i = 0; i < suspectPassports.Count; i++)
                    {
                        Console.WriteLine(suspectPassports[i].birth + ' ' + suspectPassports[i].fname + ' ' + suspectPassports[i].mname + ' ' + suspectPassports[i].lname);
                    }
                    Console.WriteLine("Выручка: " + total.ToString());
                }
                #endregion
            }
        }
    };

    public class Passport
    {
        public List<string> errorsStrings { get; set; }
        public string birth { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public string lname { get; set; }
        public string city { get; set; }
        public Passport(string Birth, string Fname, string Mname, string Lname, string City)
        {
            birth = Birth;
            fname = Fname;
            mname = Mname;
            lname = Lname;
            city = City;
        }
    };
}
