using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddIngredientsToShoppingList_ExistingRecipe_AddsCorrectly()
    {
        var manager = CreateManager();
        int addedCount = manager.AddIngredientsToShoppingList(10);

        Assert.Equal(1, addedCount);
        Assert.Equal(1, manager.ShoppingItemCount);
        Assert.Equal("1 apple", manager.GetShoppingList()[0]);
    }

    [Fact]
    // Test for clearing the shopping list (List<string>)
    // Verifies that ClearShoppingList removes all items from the list
    public void ClearShoppingList_AfterAddingItems_ClearsAll()
    {
        // Arrange: create manager and add ingredients to shopping list
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);

        // Act: clear all items from the shopping list
        manager.ClearShoppingList();

        // Assert: item count is 0 and the list is empty
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }


    [Fact]
    // Test for duplicate recipe ID in the Dictionary catalogue
    // Verifies that adding a recipe with an existing ID fails and keeps count unchanged
    public void AddRecipe_DuplicateId_ReturnsFalse()
    {
        // Arrange: create manager with recipe ID 10 already existing
        var manager = CreateManager();
        var duplicateRecipe = new Recipe { Id = 10, Title = "Duplicate Recipe" };

        // Act: try to add a recipe with the same duplicate ID
        bool result = manager.AddRecipe(duplicateRecipe);

        // Assert: add returns false, total recipe count stays at 2
        Assert.False(result);
        Assert.Equal(2, manager.RecipeCount);
    }


    [Fact]
    public void AddRecipeToCookingPlan_DuplicateRecipe_ReturnsFalse()
    {
        var manager = CreateManager();

        // First attempt to add recipe 10 to cooking plan
        bool firstAdd = manager.AddRecipeToCookingPlan(10);
        // Second attempt to add the same recipe 10 again
        bool secondAdd = manager.AddRecipeToCookingPlan(10);

        // First addition should succeed
        Assert.True(firstAdd);
        // Second addition with duplicate ID should fail
        Assert.False(secondAdd);
        // Cooking plan count should remain 1
        Assert.Equal(1, manager.CookingPlanCount);
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
