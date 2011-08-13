namespace FubuMVC.Conventions.Tests.Handlers.Posts.Category
{
    public class get_CategoryName_handler
    {
        public PostByCategoryViewModel Execute(ViewPostsByCategoryRequestModel request)
        {
            return new PostByCategoryViewModel();
        }
    }

    public class ViewPostsByCategoryRequestModel
    {
        public string CategoryName { get; set; }
    }

    public class PostByCategoryViewModel
    {
    }
}