using System;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Pokemon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string nome = textBox1.Text.ToLower();
                string apiUrl = $"https://pokeapi.co/api/v2/pokemon/{nome}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Pokemon pokemon = JsonConvert.DeserializeObject<Pokemon>(responseBody);
                        listBox1.Items.Clear();
                        listBox1.Items.Add($"Nome: {pokemon.Name.ToUpper()}");
                        listBox1.Items.Add($"Tipo: {pokemon.Types[0].Type.Name.ToUpper()}");

                        // Obter informações de habitat
                        HttpResponseMessage speciesResponse = await client.GetAsync(pokemon.Species.Url);
                        if (speciesResponse.IsSuccessStatusCode)
                        {
                            string speciesResponseBody = await speciesResponse.Content.ReadAsStringAsync();
                            PokemonSpecies pokemonSpecies = JsonConvert.DeserializeObject<PokemonSpecies>(speciesResponseBody);
                            listBox1.Items.Add($"Habitat : {pokemonSpecies.Habitat?.Name.ToUpper() ?? "Desconhecido"}");
                        }
                        else
                        {
                            listBox1.Items.Add("Habitat: Desconhecido");
                        }

                        pictureBox1.Load(pokemon.Sprites.FrontDefault);
                    }
                    else
                    {
                        MessageBox.Show($"Falha na requisição. Código de status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Pokemon não encontrado. Digite manualmente.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public class Pokemon
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("types")]
        public PokemonType[] Types { get; set; }

        [JsonProperty("sprites")]
        public Sprites Sprites { get; set; }

        [JsonProperty("species")]
        public PokemonSpeciesReference Species { get; set; }
    }

    public class PokemonType
    {
        [JsonProperty("type")]
        public TypeDetails Type { get; set; }
    }

    public class TypeDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Sprites
    {
        [JsonProperty("front_default")]
        public string FrontDefault { get; set; }
    }

    public class PokemonSpeciesReference
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class PokemonSpecies
    {
        [JsonProperty("habitat")]
        public Habitat Habitat { get; set; }
    }

    public class Habitat
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
