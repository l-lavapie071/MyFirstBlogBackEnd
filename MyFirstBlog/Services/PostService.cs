using MyFirstBlog.Helpers;
using MyFirstBlog.Entities;
using MyFirstBlog.Dtos;
using System.Text.RegularExpressions;

namespace MyFirstBlog.Services
{
    public interface IPostService
    {
        IEnumerable<PostDto> GetPosts();
        PostDto GetPost(string slug);
        PostDto CreatePost(PostDto dto);
    }

    public class PostService : IPostService
    {
        private readonly DataContext _context;

        public PostService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<PostDto> GetPosts()
        {
            return _context.Posts.Select(post => post.AsDto());
        }

        public PostDto GetPost(string slug)
        {
            var post = GetPostEntity(slug);
            return post?.AsDto();
        }

        private Post? GetPostEntity(string slug)
        {
            return _context.Posts.SingleOrDefault(p => p.Slug == slug);
        }

        public PostDto CreatePost(PostDto dto)
        {
            var post = new Post
            {
                Title = dto.Title,
                Slug = GenerateSlug(dto.Title),
                Body = dto.Body,
                CreatedDate = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            _context.SaveChanges();

            return post.AsDto();
        }

        private string GenerateSlug(string title)
        {
            var slug = Regex.Replace(title.ToLower(), @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
            return slug;
        }
    }
}
