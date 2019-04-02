using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace UserManager.Api.Controllers
{
    [ApiController]
    public abstract class BaseController
    {
        protected IMapper Mapper;

        protected BaseController(IMapper mapper)
        {
            Mapper = mapper;
        }
    }
}
