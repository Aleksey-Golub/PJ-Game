using Code.Services;
using System.Collections;
using UnityEngine;

namespace Code.Infrastructure
{
  public interface ICoroutineRunner : IService
  {
    Coroutine StartCoroutine(IEnumerator coroutine);
  }
}