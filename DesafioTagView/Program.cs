using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DesafioTagView
{
    class Program
    {
        public class Titles
        {
            public List<string> title { get; set; }

            public Titles(string json)
            {
                title = new List<string>();
                if (json != null)
                {
                    JObject jObject = JObject.Parse(json);
                    JEnumerable<JToken> lj = jObject.Children();
                    JToken token = lj.ElementAt(0);
                    lj = token.Children();
                    token = lj.ElementAt(0);
                    int ct = 0;
                    foreach (JToken element in token)
                    {
                        token = element;
                        title.Add((string)token["title"]);
                        ct++;
                    }
                }
                title.Add("");
            }
        }
        public class Ator
        {
            public string id { get; set; }
            public string name { get; set; }
            public Ator(string json)
            {
                JObject jObject = JObject.Parse(json);
                JEnumerable<JToken> lj = jObject.Children();
                JToken token = lj.ElementAt(1);
                lj = token.Children();
                token = lj.ElementAt(0);
                try {
                    token = token[0];
                    id = (string)token["id"];
                    name = (string)token["name"];
                }
                catch (Exception e) { }
            }
        }
        public class PesquisaAPITMDB
        {
            public PesquisaAPITMDB() { }
            public string pegarID(string nome)
            {
                string json;
                HttpWebRequest requerimento = (HttpWebRequest)WebRequest.Create("https://api.themoviedb.org/3/search/person?api_key=e2873e5ea3e6f966b40533aaa0d3a86d&query="+nome);
                requerimento.ContentType = "application/json; charset=utf-8";
                requerimento.Method = WebRequestMethods.Http.Get;
                requerimento.Accept = "application/json";
                var respostaJSON = (HttpWebResponse)requerimento.GetResponse();

                using (var stream = new StreamReader(respostaJSON.GetResponseStream()))
                {
                    json = stream.ReadToEnd();
                }
                return json;
            }

            public string filmesPorID(string id)
            {
                if (id != null)
                {
                    string json;
                    HttpWebRequest requerimento = (HttpWebRequest)WebRequest.Create("https://api.themoviedb.org/3/person/" + id + "/credits?api_key=e2873e5ea3e6f966b40533aaa0d3a86d");
                    requerimento.Method = WebRequestMethods.Http.Get;
                    requerimento.Accept = "application/json";
                    var respostaJSON = (HttpWebResponse)requerimento.GetResponse();
                    using (var stream = new StreamReader(respostaJSON.GetResponseStream()))
                    {
                        json = stream.ReadToEnd();
                    }
                    return json;
                }
                return null;
            } 
        }

        static void Main(string[] args)
        {
            bool rodando = true;
            while (rodando)
            {
                Console.WriteLine("API fornecida por themoviedb.org");
                Console.WriteLine("Digite 1 para a lista de filmes de um ator,");
                Console.WriteLine("2 para dois atores,");
                Console.WriteLine("3 para multiplos atores,");
                Console.WriteLine("4 para limpar a tela,");
                Console.WriteLine("e 5 para sair");
                string escolha = Console.ReadLine();
                switch (escolha) {
                    case "1":
                        Console.WriteLine("Digite o nome do ator/atriz:");
                        string nomeAtor = Console.ReadLine();
                        Ator ator = new Ator(new PesquisaAPITMDB().pegarID(nomeAtor));
                        Titles titles = new Titles(new PesquisaAPITMDB().filmesPorID(ator.id));
                        Console.WriteLine();
                        Console.WriteLine("FILMES:");
                        foreach (string title in titles.title)
                        {
                            Console.WriteLine(title);
                        }
                        break;
                    case "2":
                        Console.WriteLine("Digite o nome do 1° ator/atriz:");
                        string nomePrimeiroAtor = Console.ReadLine();
                        Ator PrimeiroAtor = new Ator(new PesquisaAPITMDB().pegarID(nomePrimeiroAtor));
                        Titles titlesPrimeiroAtor = new Titles(new PesquisaAPITMDB().filmesPorID(PrimeiroAtor.id));
                        Console.WriteLine("Digite o nome do 2° ator/atriz:");
                        string nomeSegundoAtor = Console.ReadLine();
                        Ator SegundoAtor = new Ator(new PesquisaAPITMDB().pegarID(nomeSegundoAtor));
                        Titles titlesSegundoAtor = new Titles(new PesquisaAPITMDB().filmesPorID(SegundoAtor.id));
                        Console.WriteLine();
                        Console.WriteLine("FILMES:");
                        foreach (string title in titlesPrimeiroAtor.title)
                        {
                            if (titlesSegundoAtor.title.Contains(title)) {
                                Console.WriteLine(title);
                            }
                        }
                        break;
                    case "3":
                        Console.WriteLine("Digite o numero de atores/atrizes que voce vai utilizar:");
                        int numeroAtores = Convert.ToInt32(Console.ReadLine());
                        List<string> nomeAtores = new List<string>();
                        List<Titles> titlesAtores = new List<Titles>();
                        bool contem = true;
                        for (int ct=1;ct<=numeroAtores;ct++)
                        {
                            Console.WriteLine("ator/atrizes numero "+ct);
                            nomeAtores.Add(Console.ReadLine());
                        }
                        Parallel.ForEach(nomeAtores, atorIndividual => {
                            titlesAtores.Add(new Titles(new PesquisaAPITMDB().filmesPorID(
                                             new Ator(new PesquisaAPITMDB().pegarID(atorIndividual)).id)));
                        });
                        Console.WriteLine();
                        Console.WriteLine("FILMES:");
                        foreach (string title in titlesAtores.ElementAt(0).title)
                        {
                            for (int ct=1;ct<titlesAtores.Count;ct++)
                            {
                                if (!titlesAtores.ElementAt(ct).title.Contains(title))
                                {
                                    contem = false;
                                }
                            }
                            if (contem)
                                Console.WriteLine(title);
                            contem = true;
                        }
                        break;
                    case "4":
                        Console.Clear();
                        break;
                    case "5":
                        rodando = false;
                        break;
                    default:
                        Console.WriteLine("COMANDO INVALIDO!");
                        break;
                }
            }
        }
    }
}
