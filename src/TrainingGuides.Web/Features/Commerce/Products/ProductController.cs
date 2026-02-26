// using Microsoft.AspNetCore.Mvc;
// using TrainingGuides.Web.Commerce.Products.Services;

// namespace TrainingGuides.Web.Commerce.Products;

// public class ProductController(IProductService productService) : Controller
// {
//     [HttpGet("/Store/{productSlug}/{variantSlug?}")]
//     public async Task<IActionResult> ProductDetail(string productSlug, string? variantSlug = null)
//     {
//         var product = await productService.GetProductBySlug(productSlug);

//         if (product == null)
//         {
//             return NotFound();
//         }

//         if (productService.ProductHasVariants(product))
//         {
//             var selectedVariant = variantSlug is not null
//                 ? productService.GetVariantByCodeName(product, variantSlug) ?? productService.GetFirstVariant(product)
//                 : productService.GetFirstVariant(product);
//             var model = productService.GetViewModel(product, selectedVariant);
//             return View("~/Features/Commerce/Products/Views/ProductDetail.cshtml", model);
//         }
//         else if (productService.ProductIsVariant(product))
//         {
//             var parentProduct = await productService.GetVariantParent(product);
//             var model = productService.GetViewModel(parentProduct, product);
//             return View("~/Features/Commerce/Products/Views/ProductDetail.cshtml", model);
//         }
//         else
//         {
//             var model = productService.GetViewModel(product);
//             return View("~/Features/Commerce/Products/Views/ProductDetail.cshtml", model);
//         }
//     }

// }