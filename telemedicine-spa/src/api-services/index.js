async function postWithAuthAsync(url, payload) {
    let response = new Response();
    try {
        const init = {
            method: "POST",
            headers: {
                "content-Type": "application/json",
                authorization: "Bearer " + localStorage.getItem("access-token"),
            },
            body: JSON.stringify(payload),
        };

        response = await fetch(url, init);
        return response;
    } catch (e) {
        response.ok = false;
    } finally {
        return response;
    }
}

async function getWithAuthAsync(url) {
    let response = new Response();
    try {
        const init = {
            method: "GET",
            headers: {
                authorization: "Bearer " + localStorage.getItem("access-token"),
            },
        };

        response = await fetch(url, init);
        return response.ok;
    } catch (e) {
        response.ok = false;
    } finally {
        return response;
    }
}

async function putWithAuthAsync(url, payload) {
    let response = new Response();
    try {
        const init = {
            method: "PUT",
            headers: {
                "content-Type": "application/json",
                authorization: "Bearer " + localStorage.getItem("access-token"),
            },
            body: JSON.stringify(payload),
        };

        response = await fetch(url, init);
        return response;
    } catch (e) {
        response.ok = false;
    } finally {
        return response;
    }
}
