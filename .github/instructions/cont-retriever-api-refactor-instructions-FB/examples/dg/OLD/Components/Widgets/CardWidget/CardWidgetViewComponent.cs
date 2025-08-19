﻿using System.Linq;
using System.Threading.Tasks;

using DancingGoat.Models;
using DancingGoat.Widgets;

using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

[assembly: RegisterWidget(CardWidgetViewComponent.IDENTIFIER, typeof(CardWidgetViewComponent), "Card", typeof(CardWidgetProperties), Description = "Displays an image with a centered text.", IconClass = "icon-rectangle-paragraph")]

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Controller for card widget.
    /// </summary>
    public class CardWidgetViewComponent : ViewComponent
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.LandingPage.CardWidget";


        private readonly ImageRepository imageRepository;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;


        /// <summary>
        /// Creates an instance of <see cref="CardWidgetViewComponent"/> class.
        /// </summary>
        /// <param name="imageRepository">Repository for images.</param>
        /// <param name="currentLanguageRetriever">Retrieves preferred language name for the current request. Takes language fallback into account.</param>
        public CardWidgetViewComponent(ImageRepository imageRepository, IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.imageRepository = imageRepository;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<ViewViewComponentResult> InvokeAsync(CardWidgetProperties properties)
        {
            var languageName = currentLanguageRetriever.Get();
            var image = await GetImage(properties, languageName);

            return View("~/Components/Widgets/CardWidget/_CardWidget.cshtml", new CardWidgetViewModel
            {
                ImagePath = image?.ImageFile.Url,
                Text = properties.Text
            });
        }


        private async Task<Image> GetImage(CardWidgetProperties properties, string languageName)
        {
            var image = properties.Image.FirstOrDefault();

            if (image == null)
            {
                return null;
            }

            return await imageRepository.GetImage(image.Identifier, languageName);
        }
    }
}
