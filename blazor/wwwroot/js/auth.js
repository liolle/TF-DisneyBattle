

window.auth = async () => {
  const response = await fetch('http://localhost:5032/auth', {
    credentials: 'include'
  });
  return response.ok ? await response.json() : null;
};

window.login = async (username, password) => {
  const response = await fetch('http://localhost:5032/login', {
    method: "POST",
    credentials: 'include',
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      username: username,
      password: password
    })
  });
  return response.ok ? await response.json() : null;
};

window.register = async (username, email, password) => {
  const response = await fetch('http://localhost:5032/register', {
    method: "POST",
    credentials: 'include',
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      username: username,
      password: password,
      email: email
    })
  });

  return response.ok ? await response.json() : null;
};

window.logout = async () => {
  const response = await fetch('http://localhost:5032/logout', {
    credentials: 'include'
  });
  return "";
};

window.allPersonage = async (csrf) => {
  const response = await fetch('http://localhost:5032/personage/all', {
    credentials: 'include',
    headers: {
      "X-CSRF-TOKEN": csrf ?? ""
    }
  });
  if (!response.ok) {
    return {
      isSuccess: false,
      isFailure: true,
      errorMessage: "",
      exception: null,
      result: []
    }
  }
  const body = await response.json()
  return body;
}

window.microsoftOauth = async (code, redirect_success_uri, redirect_failure_uri) => {
  const response = await fetch('http://localhost:5032/oauth/microsoft', {
    method: "POST",
    credentials: 'include',
    headers: {
      "Content-Type": "application/json",
      "Access-Control-Allow-Origin": "http://localhost:5032"
    },
    body: JSON.stringify({
      code: code,
      redirect_success_uri: redirect_success_uri,
      redirect_failure_uri: redirect_failure_uri
    })
  });

}

window.initializeMatchService = (dotNetReference) => {
  matchServiceReference = dotNetReference;
};

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7145/match_hub")
  .build();

connection.on("Join_game", (match, player) => {
  // Call the .NET method to trigger the event
  if (matchServiceReference) {
    matchServiceReference.invokeMethodAsync("NotifyJoinGame", match, player);
  }
});

connection

connection.on("game_has_changed", () => {
  if (matchServiceReference) {
    matchServiceReference.invokeMethodAsync("GameHasChanged");
  }
});

connection.on("leave_game", () => {
  if (matchServiceReference) {
    matchServiceReference.invokeMethodAsync("NotifyLeftGame");
  }
});

connection.start().catch(err => console.error(err.toString()));


window.getConnectionId = () => {
  return connection.connection.connectionId;
};

window.searchGame = async (playerId) => {
  connection.invoke("SearchGameAsync", playerId, connection.connection.connectionId)
}

window.leaveGame = async (playerId) => {
  connection.invoke("LeaveGameAsync", playerId)
}

window.getGameState = async () => {
  let response = await connection.invoke("GetGameState")
  return response
}
