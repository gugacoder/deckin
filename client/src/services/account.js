import config from '@/config.js';

const delay = (delay, value) =>
    new Promise(resolve => setTimeout(resolve, delay, value));

// Autentica o usuário e produz uma promessa para o objeto:
//  {
//    rejected: <boolean>,    <- caso o usuário tenha sido rejeitado
//    validated: <boolean>,   <- caso o usuário seja válido
//    user: <jwt>             <- caso "validated=true"
//    fault: <string>         <- causa de uma falha ou rejeicao
//  }
function authenticate(username, password) {
  //return new Promise(resolve => {
  //  const requestOptions = {
  //    method: 'POST',
  //    headers: { 'Content-Type': 'application/json' },
  //    body: JSON.stringify({ username, password })
  //  };
//
  //  return fetch(`${config.apiUrl}/Users/Authenticate`, requestOptions)
  //    .then(response => {
  //      return response.text().then(text => {
  //        const data = text && JSON.parse(text);
  //    
  //        if (!response.ok) {
  //    
  //          // em caso de resposta 401 o usuario foi ativamente rejeitado.
  //          // se houver uma sessao aberta para ele deverá ser fechada.
  //          const rejected = (response.status === 401);
  //          const fault = (data && data.message) || response.statusText;
  //    
  //          return {
  //            rejected,
  //            fault
  //          };
  //        }
  //        
  //        // É esperado um token JWT como resposta.
  //        if (user.token) {
  //          // O usário foi validado e o token JWT foi recebido com sucesso.
  //          return {
  //            validated: true,
  //            user: data
  //          };
  //        }
  //    
  //        return {
  //          fault: 'O serviço de autenticação não se comportou como esperado.'
  //        };
  //      });
  //    })
  //  });
}

function login(username, password) {
  // authenticate(username, password).then(result => {    
  //   if (result.rejected) {
  //     // O usuário foi ativamente rejeitado. Um logout deve ser executado.
  //     logout();
  //     location.reload(true);
  //     return;
  //   }
  // 
  //   if (!result.validated)
  //     return;
  // 
  //   // Salvando o usuário autenticado na sessão.
  //   localStorage.setItem('user', JSON.stringify(result.user));
  // });
}

function logout() {
}

export default {
  login,
  logout
}
