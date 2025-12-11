import { createRoot } from 'react-dom/client'
import './index.css'
import { App } from './App'
import { BrowserRouter } from 'react-router-dom'
import 'reflect-metadata'
import { AuthenticationService } from './classes/AuthenticationService'
import { RegisterSingleton, RegisterResolver, Resolve } from '@here-mobility/micro-di';
import { GlobalEventObserver } from './classes/GlobalEventObserver'
import { UserService } from './classes/UserService'
import { ServerEventHandler } from './classes/ServerEventHandler'
import { HabitService } from './classes/HabitService'
import { HabitLogService } from './classes/HabitLogService'
import { HttpService } from './classes/HttpService'
import { AuthedHttpService } from './classes/AuthedHttpService'

const {VITE_BACKEND_URL} = import.meta.env;

RegisterSingleton(GlobalEventObserver, () => {
  const observer = new GlobalEventObserver();

  //@ts-ignore
  window.observer = observer;
  return observer;
});

RegisterResolver(HttpService, () => new HttpService());
RegisterResolver(AuthedHttpService, () => new AuthedHttpService(Resolve<HttpService>(HttpService), Resolve<GlobalEventObserver>(GlobalEventObserver)));

RegisterSingleton(AuthenticationService, () => new AuthenticationService(VITE_BACKEND_URL, Resolve<HttpService>(HttpService)));
RegisterResolver(UserService, () => new UserService(VITE_BACKEND_URL, Resolve<HttpService>(HttpService), Resolve<AuthedHttpService>(AuthedHttpService)));
RegisterSingleton("IServerEventHandler", () => new ServerEventHandler(VITE_BACKEND_URL));
RegisterResolver(HabitService, () => new HabitService(VITE_BACKEND_URL, Resolve<AuthedHttpService>(AuthedHttpService)));
RegisterResolver(HabitLogService, () => new HabitLogService(VITE_BACKEND_URL, Resolve<AuthedHttpService>(AuthedHttpService)));
RegisterResolver(UserService, () => new UserService(VITE_BACKEND_URL, Resolve<HttpService>(HttpService), Resolve<AuthedHttpService>(AuthedHttpService)));

createRoot(document.getElementById('root')!).render(
  <BrowserRouter>
    <App />
  </BrowserRouter>
)
