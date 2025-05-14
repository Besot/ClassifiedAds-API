using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlutaMartAPI.Services.Classes
{
    public class WishlistService(IUnitOfWork _unitOfWork, IResponseService _responseService) : IWishlistService
    {

        public async Task<ServiceResponse<string>> AddToWishlistAsync(AddToWishlistDTO model, UserDTO currentUser)
        {
            try
            {
                if (currentUser.BuyerId == null) return _responseService.ErrorResponse<string>("Unauthorized request");
                
                var ad = await _unitOfWork.Context.Ads.FindAsync(model.AdsId);
                if (ad == null) return _responseService.ErrorResponse<string>("Invalid Ad");

                var wishlist = await _unitOfWork.Context.Set<Wishlist>()
                    .Include(w => w.WishlistItems)
                    .FirstOrDefaultAsync(w => w.BuyerId == currentUser.BuyerId.Value);

                if (wishlist == null)
                {
                    wishlist = new Wishlist
                    {
                        BuyerId = currentUser.BuyerId.Value,
                        WishlistItems = new List<WishlistItem>()
                    };
                    await _unitOfWork.Context.AddAsync(ad);
                    await _unitOfWork.CommitAsync();
                }

                if (wishlist.WishlistItems.Any(wi => wi.AdsId == model.AdsId)) return _responseService.SuccessResponse("This Ad is already added to your wishlist");
               

                var wishlistItem = new WishlistItem
                {
                    WishlistId = wishlist.Id,
                    AdsId = model.AdsId
                };

                wishlist.WishlistItems.Add(wishlistItem);
                await _unitOfWork.CommitAsync();

            return _responseService.SuccessResponse(wishlistItem.Id.ToString());

            }
            catch (Exception ex)
            {
                return _responseService.ErrorResponse<string>($"Error adding item to wishlist: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> ClearWishlistAsync(UserDTO currentUser)
        {
            try
            {
                if (currentUser.BuyerId == null)return _responseService.ErrorResponse<string>($"You must be logged in as a buyer to manage wishlist");
                

                var wishlist = await _unitOfWork.Context.Set<Wishlist>()
                    .AsNoTracking()
                    .Include(w => w.WishlistItems)
                    .FirstOrDefaultAsync(w => w.BuyerId == currentUser.BuyerId.Value);

                if (wishlist == null || wishlist.WishlistItems.Count == 0)return _responseService.SuccessResponse("Wishlist is already empty");
                
                _unitOfWork.Context.RemoveRange(wishlist.WishlistItems);
                await _unitOfWork.CommitAsync();
                return _responseService.SuccessResponse("Wishlist cleared successfully");
        
            }
            catch (Exception ex)
            {
                return _responseService.ErrorResponse<string>($"Error clearing wishlist: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<GetWishlistDTO>> GetWishlistAsync(UserDTO currentUser)
        {
            try
            {
                if (currentUser.BuyerId == null)return _responseService.ErrorResponse<GetWishlistDTO>($"You must be logged in as a buyer to manage wishlist");
                


                var wishlist = await _unitOfWork.Context.Set<Wishlist>()
                    .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Ads)
                    .ThenInclude(a => a.Vendor)
                    .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Ads)
                    .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Ads)
                    .ThenInclude(a => a.Currency)
                    .FirstOrDefaultAsync(w => w.BuyerId == currentUser.BuyerId.Value);

                if (wishlist == null)
                {
                    wishlist = new Wishlist
                    {
                        BuyerId = currentUser.BuyerId.Value,
                        WishlistItems = new List<WishlistItem>()
                    };
                   await _unitOfWork.Context.AddAsync(wishlist);
                    await _unitOfWork.CommitAsync();
                }

                var result = new GetWishlistDTO
                {
                    Id = wishlist.Id,
                    WishlistItems = wishlist.WishlistItems.Select(wi => new GetWishlistItemDTO
                    {
                        Id = wi.Id,
                        AdsId = wi.AdsId,
                        DateAdded = wi.Created,
                        Ad = new GetAdsDTO
                        {
                            Id = wi.Ads.Id,
                            Title = wi.Ads.Title,
                            Description = wi.Ads.Description,
                            Price = wi.Ads.Price,
                            DiscountPrice = wi.Ads.DiscountPrice,
                            QuantityInStock = wi.Ads.QuantityInStock,
                            Status = wi.Ads.Status.Name(),
                            IsFeatured = wi.Ads.IsFeatured,
                            AdsType = wi.Ads.AdsType,
                            Categories = _unitOfWork.Context.AdsCategories.Select(c => new CategoryDTO
                            {
                                Id = c.Id,
                                Name = c.Category.Name
                            }).ToList(),
                            AdsCondition = wi.Ads.AdsCondition.Name(),
                            VendorId = wi.Ads.VendorId,
                            BrandName = wi.Ads.Vendor.BrandName,
                        }
                    }).ToList()
                };
                return _responseService.SuccessResponse(result);

            }
            catch (Exception ex)
            {
                return _responseService.ErrorResponse<GetWishlistDTO>($"Error retrieving wishlist: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> RemoveFromWishlistAsync(Guid wishlistItemId, UserDTO currentUser)
        {
            try
            {
                if (currentUser.BuyerId == null)return _responseService.ErrorResponse<string>($"You must be logged in as a buyer to manage wishlist");
                

                var wishlistItem = await _unitOfWork.Context.Set<WishlistItem>()
                    .Include(wi => wi.Wishlist)
                    .FirstOrDefaultAsync(wi => wi.Id == wishlistItemId);

                if (wishlistItem == null)return _responseService.ErrorResponse<string>($"Wishlist item not found");

                if (wishlistItem.Wishlist.BuyerId != currentUser.BuyerId.Value)return _responseService.ErrorResponse<string>($"You can only remove items from your own wishlist");

                if (wishlistItem.Wishlist.WishlistItems.Count == 1)
                {
                    _unitOfWork.Context.Remove(wishlistItem.Wishlist);
                }
                else
                {
                    wishlistItem.Wishlist.WishlistItems.Remove(wishlistItem);
                }   
                await _unitOfWork.CommitAsync();
                return _responseService.SuccessResponse("Item removed from wishlist successfully");
            }
            catch (Exception ex)
            {
                return _responseService.ErrorResponse<string>($"Error removing item from wishlist: {ex.Message}");
            }
        }
    }
}
