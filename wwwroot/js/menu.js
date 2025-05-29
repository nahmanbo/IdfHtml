// Updated sendRequest to handle table vs message display properly
async function sendRequest(option) {
    const output = document.getElementById("output");
    output.innerHTML = "⌛ Loading...";

    let endpoint = `/api/operation/option/${option}`;
    let headers = { "Content-Type": "text/plain" };
    let body = "";

    if (option === 5 || option === 7) {
        const id = prompt("Enter terrorist ID:");
        if (!id) return;
        body = id;
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

        const text = await response.text();
        console.log("📨 Server response:\n" + text);

        try {
            const json = JSON.parse(text);
            displayJsonAsTable(json);
        } catch (e) {
            if (text.includes("===") && text.includes("|")) {
                displayTextBlock(text);
            } else if (text.includes("===")) {
                displayTextBlock(text);
            } else {
                output.textContent = text;
            }
        }
    } catch (err) {
        output.textContent = "⚠️ Error: " + err.message;
    }
}

function displayJsonAsTable(data) {
    const output = document.getElementById("output");
    output.innerHTML = "";

    if (!Array.isArray(data) && typeof data === "object") {
        for (const key in data) {
            if (Array.isArray(data[key])) {
                output.innerHTML += `<h2>${key}</h2>`;
                output.innerHTML += buildTableFromArray(data[key]);
            }
        }
        return;
    }

    if (Array.isArray(data)) {
        output.innerHTML = buildTableFromArray(data);
        return;
    }

    output.textContent = JSON.stringify(data, null, 2);
}

function buildTableFromArray(arr) {
    if (!arr || arr.length === 0) return "<p>No data available.</p>";

    const headers = Object.keys(arr[0]);
    let html = "<table><thead><tr>";
    headers.forEach(h => {
        html += `<th>${h}</th>`;
    });
    html += "</tr></thead><tbody>";

    arr.forEach(item => {
        html += "<tr>";
        headers.forEach(h => {
            const val = item[h];
            html += `<td>${Array.isArray(val) ? val.join(", ") : val}</td>`;
        });
        html += "</tr>";
    });

    html += "</tbody></table>";
    return html;
}

function displayTextBlock(text) {
    const output = document.getElementById("output");
    const pre = document.createElement("pre");
    pre.textContent = text;
    output.innerHTML = "";
    output.appendChild(pre);
}

function promptForAmmo(weaponType) {
    const inputSection = document.getElementById("input-section");
    const label = document.getElementById("input-label");
    const input = document.getElementById("input-field");

    input.value = "";
    inputSection.style.display = "block";

    if (weaponType === "F16") {
        label.textContent = "Enter Bomb Weight (0.5 or 1):";
        input.placeholder = "0.5 or 1";
    } else if (weaponType === "Tank") {
        label.textContent = "Enter Shell Quantity (2 or 3):";
        input.placeholder = "2 or 3";
    }

    window.currentWeaponType = weaponType;
    input.focus();
}

function isValidInput(weaponType, input) {
    if (weaponType === "F16") {
        return input === "0.5" || input === "1";
    }
    if (weaponType === "Tank") {
        return input === "2" || input === "3";
    }
    return false;
}

async function submitInput() {
    const inputField = document.getElementById("input-field");
    const input = inputField.value.trim();
    const weaponType = window.currentWeaponType;

    if (!isValidInput(weaponType, input)) {
        alert("⚠️ Invalid input. Please try again.");
        inputField.value = "";
        inputField.focus();
        return;
    }

    try {
        const response = await fetch("/api/operation/weapon-use", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                weaponType,
                input
            })
        });

        const result = await response.text();
        document.getElementById("output").innerHTML = result;
        document.getElementById("input-section").style.display = "none";
    } catch (error) {
        document.getElementById("output").innerText = "⚠️ Error: " + error.message;
    }
}
