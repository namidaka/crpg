export interface AsyncData<T> {
  done: boolean;
  data?: T;
}

export interface AsyncFunction<T> extends Function {
  (): PromiseLike<AsyncData<T>>;
}

export async function asyncPoll<T>(
  fn: AsyncFunction<T>,
  pollInterval: number = 5 * 100,
  maxAttempts: number = 4
): Promise<T> {
  let attempts = 0;
  const checkCondition = (resolve: Function, reject: Function): void => {
    Promise.resolve(fn())
      .then(result => {
        attempts++;
        if (result.done) {
          resolve(result.data);
        } else if (attempts < maxAttempts) {
          setTimeout(checkCondition, pollInterval, resolve, reject);
        } else {
          reject(new Error('AsyncPoller: Exceeded max attempts'));
        }
      })
      .catch(err => {
        reject(err);
      });
  };
  return new Promise(checkCondition);
}
