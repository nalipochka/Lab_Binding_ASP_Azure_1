using Azure.Storage.Blobs;
using Lab_Binding_ASP_Azure_1.Data;
using Lab_Binding_ASP_Azure_1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs.Models;
using Lab_Binding_ASP_Azure_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_Binding_ASP_Azure_1.Controllers
{
    public class PhotoController : Controller
    {
        private readonly PhotoContext context;
        private readonly BlobServiceClient blobServiceClient;
        private readonly IConfiguration configuration;
        private readonly string containerName;

        public PhotoController(PhotoContext context, BlobServiceClient blobServiceClient,
            IConfiguration configuration)
        {
            this.context = context;
            this.blobServiceClient = blobServiceClient;
            this.configuration = configuration;
            containerName = configuration.GetSection("BlobContainerName").Value;
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<Photo> photos = this.context.Photos;
            return View(await photos.ToListAsync());
        }

        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePhotoDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return View(dto);
            }
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
            string filename = $"{Path.GetFileNameWithoutExtension(dto.Photo.FileName)}" +
                $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
            BlobClient client =  containerClient.GetBlobClient(filename);
            await client.UploadAsync(dto.Photo.OpenReadStream());
            Photo addedPhoto = new Photo
            {
                Filename = filename,
                PhotoUrl = client.Uri.AbsoluteUri,
                Description = dto.Description,
            };
            context.Photos.Add(addedPhoto);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Get: Photo/Delete/5
        public async Task <IActionResult> Delete(int? id)
        {
            if(id == null || context.Photos == null)
            {
                return NotFound();
            }

            var photo = await context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            if(photo == null)
            {
                return NotFound();
            }
            return View(photo);
        }

        // POST: Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(context.Photos == null)
            {
                return Problem("Entity set 'PhotoContext.Photos'  is null.");
            }

            var photo = await context.Photos.FindAsync(id);
            if(photo != null)
            {
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(photo.Filename);
                await blobClient.DeleteIfExistsAsync();

                context.Photos.Remove(photo);
            }
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Photo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null || context.Photos == null)
            {
                return NotFound();  
            }

            var photo = await context.Photos.FindAsync(id);
            if(photo == null)
            {
                return NotFound();
            }

            var editPhotoDTO = new EditPhotoDTO
            {
                Id = photo.Id,
                Filename = photo.Filename,
                PhotoUrl = photo.PhotoUrl,
                Description = photo.Description
            };
            return View(editPhotoDTO);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPhotoDTO dto)
        {
            if(id != dto.Id)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return View(dto);   
            }

            var photo = await context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            try
            {
                photo.Description = dto.Description;
                context.Update(photo);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            catch (DbUpdateConcurrencyException)
            {
                if(!PhotoExists(dto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool PhotoExists(int id)
        {
            return context.Photos.Any(p => p.Id == id);
        }
    }
}
