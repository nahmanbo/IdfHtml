async function sendRequest(option) {
    let endpoint = `/api/operation/option/${option}`;
    let headers = {};
    let body = null;

    if (option === 5 || option === 7) {
        const name = prompt("Enter terrorist name:");
        body = name;
        headers["Content-Type"] = "text/plain";
    } else if (option === 9) {
        const targetType = prompt("Enter target type:");
        body = targetType;
        headers["Content-Type"] = "text/plain";
    } else {
        headers["Content-Type"] = "text/plain";
        body = "";
    }

    const response = await fetch(endpoint, {
        method: 'POST',
        headers,
        body
    });

    const result = await response.text();
    document.getElementById("output").textContent = result;
}
