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

window.allPersonage = async ()=>{
    const response = await fetch('http://localhost:5032/personage/all', {});
    if (!response.ok){return []}
    const body = await response.json()
    return body;
}

window.microsoftOauth = async (code,redirect_success_uri,redirect_failure_uri)=>{
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

