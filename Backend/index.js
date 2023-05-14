const express = require("express");
const {Configuration, OpenAIApi} = require("openai");
const cors = require("cors");

const start_promt =
    "Imagine you are in the RPG game, in fantasy environment where player in going throught some quests, mostly logical quizes. And you are act as his nice and loyal Flying Cat companion, who he can speak to, chat, ask for help, etc." +
    "Starting from the next message I will send you a player messages and you should response fully in the role of companion, keep game setting and give player wise advises." +
    'Limit your responses to not more then 4 senteses. Respond in format "Flying Cat: <text>".' +
    "Let's start!" +
    'Player: "Hey!"';

const OPENAI_API_KEY = "";

const app = express();
const port = 3007;

const configuration = new Configuration({
    apiKey: OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

const model_setup = {
    model: "gpt-3.5-turbo",
    completions: 1,
    maxTokens: 2200,
};

// array of messages history stored by user Id
const history = [];

const corsOptions = {
    origin: "*",
    methods: "GET,HEAD,PUT,PATCH,POST,DELETE",
};
app.use(cors(corsOptions));
app.use(express.json());

async function chat(userId, message) {
    if (!history[userId]) {
        history[userId] = [];
    }
    history[userId].push({role: "user", content: message});

    const response = await openai.createChatCompletion({
        model: model_setup.model,
        messages: history[userId],
        max_tokens: model_setup.maxTokens,
    });

    const response_text = response.data.choices[0].message.content;

    history[userId].push({role: "assistant", content: response_text});

    return response_text;
}

async function startDialog(userID) {
    history[userID] = [];

    return chat(userID, start_promt);
}

// Generate start endpoint
app.post("/start", async (req, res) => {
    try {
        const userID = history.length;
        const response = await startDialog(userID);
        res.status(200).json({response, userID});
    } catch (error) {
        console.error(error);
        res.status(500).send("Error starting conversation");
    }
});

app.post("/chat/:userId", async (req, res) => {
    try {
        const userId = req.params.userId;
        const message = `Player: "${req.body.message}" Remember to limit your responses to not more then 4 senteses. Respond in format \"Flying Cat: <text>\".`;
        const response = await chat(userId, message);
        res.status(200).json({response: response});
    } catch (error) {
        console.error(error);
        res.status(500).send("Error continuing conversation");
    }
});

// Start server
app.listen(port, () => {
    console.log(`Server listening at http://localhost:${port}`);
});

// Export the Express API
module.exports = app;
