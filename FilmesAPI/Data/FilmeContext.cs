using FilmesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmesAPI.Data;

public class FilmeContext : DbContext
{
    public FilmeContext(DbContextOptions<FilmeContext> opts) //construtor: Recebe como padrão as opções de acesso ao BD
        : base(opts) //fazendo a passagem das opções para o construtor da classe que estamos extendendo
    {
            
    }

    //criando propriedade de acesso
    public DbSet<Filme> Filmes{ get; set; }
}
