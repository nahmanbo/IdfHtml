const targetTypesThatRequireWeight = ["buildings", "open areas"];
let currentPendingRequest = null;
let pendingBatch = [];

async function sendRequest(option) {
    document.getElementById("output").innerHTML = "";  // מנקה את התצוגה הקודמת

    const headers = { "Content-Type": "text/plain" };
    let endpoint = `/api/operation/option/${option}`;
    let body = "";

    if (option === 5 || option === 7) {
        const id = prompt("Enter terrorist ID:");
        if (!id) return;
        body = id;
    } else if (option === 9) {
        const targetType = prompt("Enter target type:").trim().toLowerCase();
        if (!targetType) return;
        body = targetType;
    }

    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers,
            body
        });

        const text = await response.text();
        handleServerResponse(text);
    } catch (err) {
        document.getElementById("output").textContent = "⚠️ Error: " + err.message;
    }
}

function handleServerResponse(text) {
    try {
        const parsed = JSON.parse(text);

        if (Array.isArray(parsed)) {
            if (parsed[0] === "🕓 Ammo Input Required") {
                currentPendingRequest = parsed[2];
                showInputBox("Enter ammo amount:");
                return;
            }

            if (parsed[0] === "🕓 Batch Ammo Input Required") {
                pendingBatch = parsed[2];
                showNextInBatch();
                return;
            }

            renderResponse(text);
            return;
        }

        appendResponse(text);
    } catch {
        document.getElementById("output").textContent = text;
    }
}

function showNextInBatch() {
    if (pendingBatch.length === 0) {
        document.getElementById("input-section").style.display = "none";
        return;
    }
    currentPendingRequest = pendingBatch.shift();
    showInputBox(`Enter ammo for target ID ${currentPendingRequest.Id} (${currentPendingRequest.Target})`);
}

function showInputBox(labelText) {
    document.getElementById("input-label").textContent = labelText;
    document.getElementById("input-section").style.display = "flex";
    document.getElementById("input-field").value = "";
}

async function submitInput() {
    const field = document.getElementById("input-field");
    const value = parseFloat(field.value);

    const { Id, Target } = currentPendingRequest;
    const validAmmoByTarget = {
        "buildings": [0.5, 1],
        "open areas": [2, 3]
    };
    const allowed = validAmmoByTarget[Target] || [];
    if (!allowed.includes(value)) {
        alert(`Invalid input for target '${Target}'. Allowed values: ${allowed.join(" or ")}`);
        return;
    }

    const payload = { Id, Target, Ammo: value };

    try {
        const response = await fetch("/api/operation/option/10", {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const text = await response.text();
        appendResponse(text);
        showNextInBatch();
    } catch (err) {
        document.getElementById("output").textContent = "⚠️ Error: " + err.message;
    }
}

function appendResponse(text) {
    const output = document.getElementById("output");
    try {
        const parsed = JSON.parse(text);

        if (Array.isArray(parsed) && parsed.length === 3) {
            const [title, subtitle, data] = parsed;
            const html = `
                <div class="response-block">
                    <h2>${title}</h2>
                    <h4>${subtitle}</h4>
                    ${Array.isArray(data) && data.length > 0 ? buildTable(data) : "<p><em>No data</em></p>"}
                    <hr/>
                </div>
            `;
            output.innerHTML += html;
        } else {
            output.innerHTML += `<pre>${JSON.stringify(parsed, null, 2)}</pre><hr/>`;
        }
    } catch {
        output.innerHTML += `<pre>${text}</pre><hr/>`;
    }
}

function renderResponse(text) {
    const output = document.getElementById("output");
    try {
        const parsed = JSON.parse(text);

        if (Array.isArray(parsed)) {
            if (parsed.length % 3 === 0 && typeof parsed[0] === "string" && typeof parsed[1] === "string") {
                displaySequentialTriplets(parsed);
            } else if (parsed.length === 3 && typeof parsed[0] === "string" && isDictionaryOfArrays(parsed[2])) {
                displayTripletWithGroupedTables(parsed);
            } else if (parsed.length === 3 && typeof parsed[0] === "string" && Array.isArray(parsed[2])) {
                displayTripletWithArray(parsed);
            } else if (parsed.every(item => Array.isArray(item) && item.length === 3)) {
                displayMultipleTriplets(parsed);
            } else {
                output.innerHTML = `<pre>${JSON.stringify(parsed, null, 2)}</pre>`;
            }
        } else {
            output.innerHTML = `<pre>${JSON.stringify(parsed, null, 2)}</pre>`;
        }
    } catch {
        output.textContent = text;
    }
}

function clearOutput() {
    document.getElementById("output").innerHTML = "🧭 Output cleared.";
}

function isDictionaryOfArrays(obj) {
    if (!obj || typeof obj !== "object" || Array.isArray(obj)) return false;
    return Object.values(obj).every(value => Array.isArray(value));
}

function displayTripletWithArray([title, subtitle, data]) {
    const output = document.getElementById("output");
    output.innerHTML = `<h2>${title}</h2><h4>${subtitle}</h4>`;
    output.innerHTML += Array.isArray(data) && data.length > 0 ? buildTable(data) : `<p><em>No data</em></p>`;
}

function displayTripletWithGroupedTables([title, subtitle, grouped]) {
    const output = document.getElementById("output");
    output.innerHTML = `<h2>${title}</h2><h4>${subtitle}</h4>`;
    for (const group in grouped) {
        output.innerHTML += `<h3>${group}</h3>`;
        output.innerHTML += Array.isArray(grouped[group]) && grouped[group].length > 0
            ? buildTable(grouped[group])
            : `<p><em>No data for category</em></p>`;
    }
}

function displayMultipleTriplets(triplets) {
    const output = document.getElementById("output");
    output.innerHTML = "";
    triplets.forEach(([title, subtitle, data], i) => {
        output.innerHTML += `<h2>${title}</h2><h4>${subtitle}</h4>`;
        output.innerHTML += Array.isArray(data) && data.length > 0 ? buildTable(data) : `<p><em>No data</em></p>`;
        if (i < triplets.length - 1) output.innerHTML += `<hr/>`;
    });
}

function displaySequentialTriplets(flatArray) {
    const output = document.getElementById("output");
    output.innerHTML = "";
    for (let i = 0; i < flatArray.length; i += 3) {
        const title = flatArray[i];
        const subtitle = flatArray[i + 1];
        const data = flatArray[i + 2];

        output.innerHTML += `<h2>${title}</h2><h4>${subtitle}</h4>`;
        if (Array.isArray(data)) {
            output.innerHTML += buildTable(data);
        } else if (typeof data === "object" && data !== null) {
            for (const category in data) {
                output.innerHTML += `<h3>${category}</h3>`;
                output.innerHTML += buildTable(data[category]);
            }
        } else {
            output.innerHTML += `<p><em>No data</em></p>`;
        }
        if (i + 3 < flatArray.length) output.innerHTML += `<hr/>`;
    }
}

function buildTable(dataArray) {
    if (!Array.isArray(dataArray) || dataArray.length === 0) return "<p>No data available.</p>";
    const headers = Object.keys(dataArray[0]);
    let html = "<table><thead><tr>" + headers.map(h => `<th>${h}</th>`).join("") + "</tr></thead><tbody>";
    dataArray.forEach(row => {
        html += "<tr>" + headers.map(h => `<td>${Array.isArray(row[h]) ? row[h].join(", ") : row[h] ?? ""}</td>`).join("") + "</tr>";
    });
    html += "</tbody></table>";
    return html;
}
