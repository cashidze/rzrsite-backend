﻿using Newtonsoft.Json;
using RzrSite.Admin.Helper;
using RzrSite.Admin.Repositories.Interfaces;
using RzrSite.Models.Entities;
using RzrSite.Models.Resources.FeatureType;
using RzrSite.Models.Responses.FeatureType;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RzrSite.Admin.Repositories
{
  public class FeatureTypeRepository : IFeatureTypeRepository
  {
    HttpClient _client = new HttpClient(); 

    public async Task<AddedFeatureType> AddFeatureType(int categoryId, PostFeatureType postModel)
    {
      var stringifiedObject = JsonConvert.SerializeObject(postModel);
      var response = await _client.PostAsync($"{UrlLocator.ApiUrl}/Category/{categoryId}/FeatureType", new StringContent(stringifiedObject, Encoding.Default, "application/json"));
      if (response.IsSuccessStatusCode)
      {
        var resultString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AddedFeatureType>(resultString);
      };

      return null;
    }

    public async Task<FeatureType> GetFeatureType(int categoryId, int id)
    {
      var response = await _client.GetAsync($"{UrlLocator.ApiUrl}/Category/{categoryId}/FeatureType/{id}");
      if (response.IsSuccessStatusCode)
      {
        var resultString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<FeatureType>(resultString);
      }

      return null;
    }

    public async Task<IList<FeatureType>> GetAllFeatureTypes(int categoryId)
    {
      var response = await _client.GetAsync($"{UrlLocator.ApiUrl}/Category/{categoryId}/FeatureType");
      if (response.IsSuccessStatusCode)
      {
        var resultString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IList<FeatureType>>(resultString);
      }

      return null;
    }

    public async Task<bool> RemoveFeatureType(int categoryId, int id)
    {
      var response = await _client.DeleteAsync($"{UrlLocator.ApiUrl}/Category/{categoryId}/FeatureType/{id}");
      if (response.IsSuccessStatusCode)
      {
        return true;
      }

      return false;
    }

    public async Task<FeatureType> UpdateFeatureType(int categoryId, int id, PutFeatureType putModel)
    {
      var stringifiedObject = JsonConvert.SerializeObject(putModel);
      var response = await _client.PutAsync($"{UrlLocator.ApiUrl}/Category/{categoryId}/FeatureType/{id}", new StringContent(stringifiedObject, Encoding.Default, "application/json"));
      if (response.IsSuccessStatusCode)
      {
        var resultString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<FeatureType>(resultString);
      }

      return null;
    }
  }
}
