using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Books
{
    public class BookList
    {
        public class Query : IRequest<List<Book>> {}
        

            public class Handler : IRequestHandler<Query, List<Book>>
            {
                private readonly DataContext _context;
                public Handler(DataContext context)
                {
                    _context = context;
                }

                public async Task<List<Book>> Handle(Query request, CancellationToken cancellationToken)
                {
                    return await _context.Book.ToListAsync();
                }
            }
    }
}
