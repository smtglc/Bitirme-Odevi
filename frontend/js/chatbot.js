// Chatbot functionality
class Chatbot {
    constructor() {
        this.isOpen = false;
        this.messageHistory = [];
        this.init();
    }

    init() {
        this.createChatbotHTML();
        this.attachEventListeners();
    }

    createChatbotHTML() {
        const chatbotHTML = `
            <div class="chatbot-container">
                <button id="chatbot-toggle" class="chatbot-btn">
                    <i class="fas fa-robot"></i>
                </button>
                <div id="chatbot-window" class="chatbot-window hidden">
                    <div class="chatbot-header">
                        <h4><i class="fas fa-robot"></i> AI Asistan</h4>
                        <button id="chatbot-close"><i class="fas fa-times"></i></button>
                    </div>
                    <div id="chatbot-messages" class="chatbot-messages">
                        <div class="chat-welcome">
                            <i class="fas fa-robot"></i>
                            <h3>Merhaba! 👋</h3>
                            <p>Size nasıl yardımcı olabilirim?</p>
                            <p style="font-size: 0.8rem; margin-top: 1rem; color: #999;">
                                Randevularınız, hizmetleriniz hakkında soru sorabilirsiniz.
                            </p>
                        </div>
                    </div>
                    <div class="chatbot-input-container">
                        <input type="text" id="chatbot-input" placeholder="Bir şey sorun..." />
                        <button id="chatbot-send" class="chatbot-send-btn">
                            <i class="fas fa-paper-plane"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', chatbotHTML);
    }

    attachEventListeners() {
        const toggleBtn = document.getElementById('chatbot-toggle');
        const closeBtn = document.getElementById('chatbot-close');
        const sendBtn = document.getElementById('chatbot-send');
        const input = document.getElementById('chatbot-input');

        toggleBtn.addEventListener('click', () => this.toggle());
        closeBtn.addEventListener('click', () => this.close());
        sendBtn.addEventListener('click', () => this.sendMessage());
        input.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') this.sendMessage();
        });
    }

    toggle() {
        const window = document.getElementById('chatbot-window');
        this.isOpen = !this.isOpen;

        if (this.isOpen) {
            window.classList.remove('hidden');
            document.getElementById('chatbot-input').focus();
        } else {
            window.classList.add('hidden');
        }
    }

    close() {
        this.isOpen = false;
        document.getElementById('chatbot-window').classList.add('hidden');
    }

    async sendMessage() {
        const input = document.getElementById('chatbot-input');
        const message = input.value.trim();

        if (!message) return;

        // Clear input
        input.value = '';

        // Add user message to UI
        this.addMessage('user', message);

        // Show typing indicator
        this.showTypingIndicator();

        try {
            // Send to API
            const response = await api.chat.sendMessage(message, this.messageHistory);

            // Remove typing indicator
            this.hideTypingIndicator();

            if (response.success) {
                const botMessage = response.data.message;
                this.addMessage('model', botMessage);

                // Update history
                this.messageHistory.push({
                    role: 'user',
                    content: message,
                    timestamp: new Date().toISOString()
                });
                this.messageHistory.push({
                    role: 'model',
                    content: botMessage,
                    timestamp: response.data.timestamp
                });

                // Keep only last 10 messages in history
                if (this.messageHistory.length > 20) {
                    this.messageHistory = this.messageHistory.slice(-20);
                }
            } else {
                this.addMessage('model', 'Üzgünüm, bir hata oluştu. Lütfen tekrar deneyin.');
            }
        } catch (error) {
            this.hideTypingIndicator();
            this.addMessage('model', 'Bağlantı hatası. Lütfen tekrar deneyin.');
            console.error('Chat error:', error);
        }
    }

    addMessage(role, content) {
        const messagesContainer = document.getElementById('chatbot-messages');

        // Remove welcome message if exists
        const welcome = messagesContainer.querySelector('.chat-welcome');
        if (welcome) welcome.remove();

        const messageDiv = document.createElement('div');
        messageDiv.className = `chat-message ${role}`;

        const avatar = document.createElement('div');
        avatar.className = 'chat-message-avatar';
        avatar.innerHTML = role === 'user' ? '<i class="fas fa-user"></i>' : '<i class="fas fa-robot"></i>';

        const contentDiv = document.createElement('div');
        contentDiv.className = 'chat-message-content';
        contentDiv.textContent = content;

        messageDiv.appendChild(avatar);
        messageDiv.appendChild(contentDiv);

        messagesContainer.appendChild(messageDiv);
        this.scrollToBottom();
    }

    showTypingIndicator() {
        const messagesContainer = document.getElementById('chatbot-messages');
        const typingDiv = document.createElement('div');
        typingDiv.id = 'typing-indicator';
        typingDiv.className = 'chat-message model';
        typingDiv.innerHTML = `
            <div class="chat-message-avatar">
                <i class="fas fa-robot"></i>
            </div>
            <div class="typing-indicator">
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
            </div>
        `;
        messagesContainer.appendChild(typingDiv);
        this.scrollToBottom();
    }

    hideTypingIndicator() {
        const indicator = document.getElementById('typing-indicator');
        if (indicator) indicator.remove();
    }

    scrollToBottom() {
        const messagesContainer = document.getElementById('chatbot-messages');
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
}

// Initialize chatbot when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        new Chatbot();
    });
} else {
    new Chatbot();
}
