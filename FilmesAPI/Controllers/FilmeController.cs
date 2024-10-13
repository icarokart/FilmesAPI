//using Microsoft.AspNetCore.Components;
using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
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





    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
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

    /// <summary>
    /// Retorna um numero de filmes do banco de dados, pré especificado pelo parametro "take"
    /// </summary>
    /// <param name="readingFilmeDto">parametros necessários para retorno dos filmes</param>
    /// <returns>IEnumerable</returns>
    /// <response code="200">Caso o retorno seja feito com sucesso</response>
    [HttpGet]
    public IEnumerable<ReadingFilmeDto> RecuperaFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50) 
    {
        // return filmes.Skip(skip).Take(take);
        //return _context.Filmes.Skip(skip).Take(take);
        return _mapper.Map<List<ReadingFilmeDto>>(_context.Filmes.Skip(skip).Take(take));


    }

    //RETORNA UM FILME PELO ID
    [HttpGet("{id}")] //bind obrigatória referenciando o parametro passado na URL
    public IActionResult RecuperaFilmePorId(int id) // IActionResult: define um contrato para os diferentes tipos de resposta que um controlador pode retornar
    {
        //var filme = filmes.FirstOrDefault(filme => filme.Id == id); //para cada elemento da lista de filmes, para o filme que eu estou iterando, eu quero que o Id dele seja igual ao ID informado por parametro
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id); //para cada elemento da lista de filmes, para o filme que eu estou iterando, eu quero que o Id dele seja igual ao ID informado por parametro

        if (filme == null) return NotFound(); // se o filme nao existir, retorno um 404
        
        var filmeDto = _mapper.Map<ReadingFilmeDto>(filme);

        return Ok(filmeDto); //senão, retorno um 200 junto das informações do filme

    }

    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null) return NotFound();

        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();

        return NoContent();

    }


    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null) return NotFound();

        //convertendo o filme retornado do banco para um "UpdateFilmeDto" para conseguir aplicar as validações
        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        //se a tentativa mudança que está sendo feita for aplicada no "filmeParaAtualizar" e ele não obtiver um "modelState" válido
        if (!TryValidateModel(filmeParaAtualizar))
        {
            //descarto as alterações e retorno erro
            return ValidationProblem(ModelState);
        }

        //caso contrário, salvo as alterações convertendo-o de volta para um "Filme"...
        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null) return NotFound();

        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
