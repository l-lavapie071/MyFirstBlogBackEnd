using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MyFirstBlog.Controllers;
using MyFirstBlog.Services;
using MyFirstBlog.Dtos;
using System;
using System.Collections.Generic;

public class PostsControllerTests
{
    private readonly Mock<IPostService> _mockService;
    private readonly PostsController _controller;

    public PostsControllerTests()
    {
        _mockService = new Mock<IPostService>();
        _controller = new PostsController(_mockService.Object);
    }

    [Fact]
    public void CreatePost_ValidInput_ReturnsCreatedResponse()
    {
        var input = new PostDto { Title = "some title", Body = "some content" };
        var expected = new PostDto { Title = "some title", Body = "some content", Slug = "some-title", CreatedDate = DateTime.UtcNow };

        _mockService.Setup(s => s.CreatePost(input)).Returns(expected);

        var result = _controller.PostCreateDto(input);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var post = Assert.IsType<PostDto>(createdResult.Value);

        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(expected.Title, post.Title);
        Assert.Equal(expected.Body, post.Body);
    }

    [Fact]
    public void CreatePost_BlankTitle_ReturnsBadRequest()
    {
        var input = new PostDto { Title = "", Body = "some content" };

        var result = _controller.PostCreateDto(input);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errorObj = badRequest.Value;
        var errorsProperty = errorObj.GetType().GetProperty("errors");
        var errors = errorsProperty.GetValue(errorObj) as string[];

        Assert.Contains("Title cannot be blank", errors);
    }
}
