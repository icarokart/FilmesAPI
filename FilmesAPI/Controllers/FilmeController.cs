//using Microsoft.AspNetCore.Components;
using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[Controller]")]

public class FilmeController : ControllerBase
{
    /*
    GUARDA OS DADOS EM TEMPO DE EXECUÇÃO NA MEMORIA - NÃO MAIS NECESSÁRIO POIS VAMOS UTILIZAR O CONTEXTO DO ENTITY PARA SALVÁ-LOS NO BANCO DE DADOS 
    
    private static List<Filme> filmes = new List<Filme>();

    private static int id = 0;
    */

    //Instanciando os construtores da classe
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }





    //CADASTRAR NOVO FILME
    [HttpPost]
    //public IActionResult AdicionaFilme([FromBody] Filme filme) --> DESTA FORMA ELE UTILIZA DIRETO DA MODEL
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto) // --> UTILIZANDO UM DTO (DATA TRANSFER OBJECT)
    {
        /* UTILIZADO ANTERIORMENTE PARA GUARDAR OS DADOS EM TEMPO DE EXECUÇÃO - LÓGICA SUBSTITUIDA PARA SALVA-LOS NO BD
        filme.Id = id++;
        filmes.Add(filme);
        */

        Filme filme = _mapper.Map<Filme>(filmeDto);

        _context.Filmes.Add(filme); //cria o filme no banco de dados
        _context.SaveChanges(); //salva as alterações

        return CreatedAtAction(nameof(RecuperaFilmePorId), 
                               new { id = filme.Id },
                               filme); //Após a criação bem sucedida de um filme, aciono a rota GET passando o ID, para retornar as informações do filme criado.

    }


    /*
    1 - utilizando o IEnumerable para garantir a maxima abstração. Se no futuro as propriedades da minha lista mudar, eu nao preciso atualizar este método.
    2 - Utilizando "skip" e "take" para limitar o numero de resultados a serem retornados (PAGINAÇÃO DE RESULTADOS)
    3 - Metodo Skip: Determina qual o tamanho do "salto" inicial a ser realizado
    4 - Metodo Take: Determina a quantidade de itens a ser retornados após o salto
    5 - Parametro [FromQuuery]: Informações passadas diretamente na url após a "?"
    6 - Valores padrões nos parametros "skip" e "take": Determinado para caso o usuario não passe nenhuma informações de paginação na url ("Pula" 0 itens e "pega" 50 itens após o pulo)
    */
    [HttpGet]
    public IEnumerable<Filme> RecuperaFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50) 
    {
       // return filmes.Skip(skip).Take(take);
        return _context.Filmes.Skip(skip).Take(take);

    }

    //RETORNA UM FILME PELO ID
    [HttpGet("{id}")] //bind obrigatória referenciando o parametro passado na URL
    public IActionResult RecuperaFilmePorId(int id) // IActionResult: define um contrato para os diferentes tipos de resposta que um controlador pode retornar
    {
        //var filme = filmes.FirstOrDefault(filme => filme.Id == id); //para cada elemento da lista de filmes, para o filme que eu estou iterando, eu quero que o Id dele seja igual ao ID informado por parametro
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id); //para cada elemento da lista de filmes, para o filme que eu estou iterando, eu quero que o Id dele seja igual ao ID informado por parametro

        if (filme == null) return NotFound(); // se o filme nao existir, retorno um 404
        
        return Ok(filme); //senão, retorno um 200 junto das informações do filme

    }
}
