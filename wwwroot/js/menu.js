
async function sendRequest(option) {
    const output = document.getElementById("output");
    output.innerHTML = "⌛ Loading...";

    let endpoint = `/api/operation/option/${option}`;
    let headers = { "Content-Type": "text/plain" };
    let body = "";

    if (option === 5 || option === 7) {
        const name = prompt("Enter terrorist name:");
        if (!name) return;
        body = name;
    } else if (option === 9) {
        const targetType = prompt("Enter target type:");
        if (!targetType) return;
        body = targetType;
    }

    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers,
            body
        });

        const result = await response.text();
        console.log("📨 Server response:\n" + result);

        if (result.includes("===") && result.includes("|")) {
            displayAllTables(result);
        } else if (result.includes("===")) {
            displayReport(result);
        } else {
            output.textContent = result;
        }
    } catch (err) {
        output.textContent = "⚠️ Error: " + err.message;
    }
}

function displayAllTables(text) {
    const output = document.getElementById("output");
    const blocks = text.trim().split(/(?=^=== )/m);
    let finalHtml = "";

    for (const block of blocks) {
        if (block.startsWith("=== ") && block.includes("|")) {
            const lines = block.trim().split("\n");
            const title = lines[0].replace(/=+/g, '').trim();
            const headers = lines[1].split("|").map(h => h.trim());

            let html = `<h2>${title}</h2><table><thead><tr>`;
            headers.forEach(header => {
                html += `<th>${header}</th>`;
            });
            html += "</tr></thead><tbody>";

            for (let i = 2; i < lines.length; i++) {
                const row = lines[i].split("|").map(c => c.trim());
                html += "<tr>" + row.map(cell => `<td>${cell}</td>`).join("") + "</tr>";
            }

            html += "</tbody></table>";
            finalHtml += html + "<br/>";
        } else {
            finalHtml += `<p>${block}</p>`;
        }
    }

    output.innerHTML = finalHtml;
}

function displayReport(text) {
    const output = document.getElementById("output");
    const lines = text.trim().split("\n");

    const titleLine = lines[0].startsWith("===")
        ? `<h2>${lines[0].replace(/=+/g, '').trim()}</h2>`
        : "";

    let table = "<table><tbody>";

    for (let i = 1; i < lines.length; i++) {
        const line = lines[i].trim();
        const [label, value] = line.split(/:(.+)/).map(s => s?.trim());
        if (value) {
            table += `<tr><th>${label}</th><td>${value}</td></tr>`;
        } else {
            table += `<tr><td colspan="2">${line}</td></tr>`;
        }
    }

    table += "</tbody></table>";
    output.innerHTML = titleLine + table;
}
