using System.Linq;
using DevFitness.API.Core.Entities;
using DevFitness.API.Models.InputModels;
using DevFitness.API.Models.ViewModels;
using DevFitness.API.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace DevFitness.API.Controllers
{
  
  //api/users/4/meals controller
  [Route("api/users/{userId}/meals")]
  public class MealsController : ControllerBase
  {
    private readonly DevFitnessDbContext _dbContext;
    public MealsController(DevFitnessDbContext dbContext)
    {
      _dbContext = dbContext;
    }
     // api/users/4/meals HTTP GET 
    [HttpGet]
    public IActionResult GetAll(int userId)
    {
      var allMeals = _dbContext.Meals.Where(m => m.UserId == userId && m.Active);

      var allMealsViewModel = allMeals
        .Select(m => new MealViewModel(
          m.Id, m.Description, m.Calories, m.Date));
      return Ok(allMealsViewModel);
    }

    // api/users/4/meals/16 HTTP GET
    [HttpGet("{mealId}")]
    public IActionResult Get(int userId, int mealId)
    {
      var meal = _dbContext.Meals.SingleOrDefault(m => m.Id == mealId && m.UserId == userId);
      
      if(meal == null)
        return NotFound();

      var mealViewModel = new MealViewModel(meal.Id, meal.Description, meal.Calories, meal.Date);
      
      return Ok(mealViewModel);
    }
    
    // api/users/4/meals HTTP POST
    [HttpPost]
    public IActionResult Post(int userId, [FromBody] CreateMealInputModel inputModel)
    {
      var meal = new Meal(inputModel.Description, inputModel.Calories, inputModel.Date, userId);

      _dbContext.Meals.Add(meal);
      _dbContext.SaveChanges();
     
      return CreatedAtAction(nameof(Get), new {userId = userId, mealId = meal.Id}, inputModel);
    }
    
    // api/users/4/meals/16 HTTP Put
    [HttpPut("{mealId}")]
    public IActionResult Put(int userId, int mealId, [FromBody] UpdateMealInputModel inputModel)
    {
      var meal = _dbContext.Meals.SingleOrDefault(m => m.Id == userId && m.Id == mealId);
      
      if(meal == null)
          return NotFound();

      meal.Update(inputModel.Description, inputModel.Calories, inputModel.Date);
      _dbContext.SaveChanges();
      return NoContent();
    }
    
    // api/users/4/meals/16 HTTP Delete
    [HttpDelete("{mealId}")]
    public IActionResult Delete(int userId, int mealId)
    {
      var meal = _dbContext.Meals.SingleOrDefault(m => m.Id == userId && m.Id == mealId);
      
      if(meal == null)
        return NotFound();
      
      meal.Deactivate();
      _dbContext.SaveChanges();
      
      return NoContent();
    }

  }
}