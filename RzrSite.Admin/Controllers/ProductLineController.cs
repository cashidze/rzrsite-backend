﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RzrSite.Admin.Repositories.Interfaces;
using RzrSite.Admin.ViewModels.ProductLines;
using RzrSite.Models.Resources.ProductLine;
using RzrSite.Models.Responses.ProductLine;
using System.Linq;
using System.Threading.Tasks;
using RzrSite.Models.Resources.Feature;

namespace RzrSite.Admin.Controllers
{
  [Route("/category/{categoryId}/[controller]")]
  public class ProductLineController : Controller
  {
	private readonly IProductLineRepository _repo;
	private readonly IMapper _mapper;
	private readonly ICategoryRepository _categoryRepo;
	private readonly IFeatureRepository _repoFeature;

	public ProductLineController(ICategoryRepository categoryRepo, IProductLineRepository repo, IMapper mapper, IFeatureRepository repoFeature)
	{
	  _repo = repo;
	  _mapper = mapper;
	  _categoryRepo = categoryRepo;
	  _repoFeature = repoFeature;
	}

	[HttpGet("/[controller]")]
	[HttpGet("/[controller]/[action]")]
	public async Task<IActionResult> Index()
	{
	  var categories = await _categoryRepo.GetCategories();
	  var vm = new IndexViewModel();
	  foreach (var category in categories)
	  {
		var pLines = await _repo.GetProductLines(category.Id);
		if (pLines != null)
		{
		  vm.ProductLines.Add(category.Id, pLines.ToList());
		}
	  }

	  return View(vm);
	}

	[HttpGet("[action]")]
	public IActionResult Add(int categoryId)
	{
	  return View(new PostProductLine());
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> Add(int categoryId, PostProductLine model)
	{
	  var response = await _repo.AddProductLine(categoryId, model);
	  if (response != null)
	  {
		return RedirectToAction("Edit", "Category", new { categoryId });
	  }

	  //TODO: Error handling
	  return RedirectToAction("Edit", "Category", new { categoryId });
	}

	[HttpGet("[action]/{id}")]
	public async Task<IActionResult> Edit(int categoryId, int id)
	{
	  var productLine = await _repo.GetProductLine(categoryId, id);
	  if (productLine == null)
	  {
		//TODO: Error handling
		return RedirectToAction("Edit", "Category", new { categoryId });
	  }

	  var strippedProductLine = _mapper.Map<StrippedProductLine>(productLine);

	  return View(strippedProductLine);
	}

	[HttpPost("[action]/{id}")]
	public async Task<IActionResult> Edit(int categoryId, int id, PutProductLine productLine)
	{
	  var response = await _repo.UpdateProductLine(categoryId, id, productLine);
	  return View(response);
	}

	[HttpGet("[action]/{id}")]
	public async Task<IActionResult> Delete(int categoryId, int id)
	{
	  var response = await _repo.RemoveProductLine(categoryId, id);
	  if (response == true)
	  {
		return RedirectToAction("Edit", "Category", new { categoryId });
	  }

	  //TODO: Error handling
	  return RedirectToAction("Edit", "Category", new { categoryId });
	}

	[HttpGet("[action]/{id}")]
	public async Task<IActionResult> FeatureTable(int categoryId, int id)
	{
	  var pLine = await _repo.GetProductLine(categoryId, id);
	  return View(pLine);
	}

	[HttpPost("[action]/{id}")]
	public async Task<IActionResult> FeatureTable(PutFeatureTableItems model)
	{
	  foreach (var item in model.Items)
	  {
		await _repoFeature.UpdateProduct(item.ProductId, item.Id, new PutFeature
		{
		  FeatureTypeId = item.TypeId,
		  Value = item.Value,
		  Weight = 0
		});
	  }
	  return RedirectToAction("Edit", "ProductLine", new { categoryId = model.CategoryId, id = model.ProductLineId });
	}

	[HttpGet("[action]")]
	public IActionResult NavigateBackwards(int categoryId)
	{
	  return RedirectToAction("Edit", "Category", new { categoryId });
	}
  }
}
