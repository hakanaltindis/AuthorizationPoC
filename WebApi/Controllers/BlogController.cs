using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly WebApiDbContext _dbContext;

        public BlogController(WebApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<string> Post(Blog blog)
        {
            if (!ModelState.IsValid)
                return JsonConvert.SerializeObject(false);

            await _dbContext.Blogs.AddAsync(blog);

            _dbContext.SaveChanges();

            return JsonConvert.SerializeObject(true);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var blogs = _dbContext.Blogs.ToList();
            var serializedList = JsonConvert.SerializeObject(blogs);
            return await Task.Run(() => serializedList);
        }

        [HttpGet("posts")]
        public async Task<string> GetPosts()
        {
            var posts = _dbContext.Posts.Include(i => i.Blog).ToList();
            return await Task.Run(() =>
                JsonConvert.SerializeObject(
                    posts,
                    Formatting.Indented,
                    new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All }));
        }
    }
}
