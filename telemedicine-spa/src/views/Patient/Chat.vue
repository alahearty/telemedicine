<template>
  <div class="chat-container">
    <div class="chat-wrapper">
      <!-- Chat Sidebar -->
      <div class="chat-sidebar">
        <div class="sidebar-header">
          <h2 class="text-xl font-bold text-gray-800 dark:text-gray-200">Messages</h2>
          <button class="new-chat-btn" @click="startNewChat">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                </svg>
          </button>
        </div>
        
        <div class="search-container">
          <div class="search-input-wrapper">
            <svg class="search-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <input 
              type="search" 
              v-model="searchQuery"
              class="search-input" 
              placeholder="Search conversations..."
            />
            </div>
          </div>

        <div class="chat-list">
          <div 
            v-for="chat in filteredChats" 
            :key="chat.id"
            :class="['chat-item', { active: selectedChat?.id === chat.id }]"
            @click="selectChat(chat)"
          >
            <div class="chat-avatar-wrapper">
              <img :src="chat.avatar" :alt="chat.name" class="chat-avatar" />
              <span v-if="chat.online" class="online-indicator"></span>
                  </div>
            <div class="chat-info">
              <div class="chat-header-info">
                <span class="chat-name">{{ chat.name }}</span>
                <span class="chat-time">{{ chat.lastMessageTime }}</span>
                </div>
              <div class="chat-preview">
                <span class="chat-message">{{ chat.lastMessage }}</span>
                <span v-if="chat.unread" class="unread-badge">{{ chat.unread }}</span>
                </div>
                  </div>
                </div>
        </div>
            </div>

      <!-- Chat Main Area -->
      <div class="chat-main">
        <div v-if="!selectedChat" class="no-chat-selected">
          <div class="no-chat-content">
            <svg class="w-24 h-24 text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
            <h3 class="text-2xl font-semibold text-gray-600 dark:text-gray-400 mb-2">Select a conversation</h3>
            <p class="text-gray-500 dark:text-gray-500">Choose a chat from the sidebar to start messaging</p>
                  </div>
                  </div>

        <div v-else class="chat-window">
          <!-- Chat Header -->
          <div class="chat-window-header">
            <div class="header-user-info">
              <div class="header-avatar-wrapper">
                <img :src="selectedChat.avatar" :alt="selectedChat.name" class="header-avatar" />
                <span v-if="selectedChat.online" class="header-online-indicator"></span>
                  </div>
              <div>
                <h3 class="header-name">{{ selectedChat.name }}</h3>
                <p class="header-status">{{ selectedChat.online ? 'Online' : 'Offline' }}</p>
                  </div>
            </div>
            <div class="header-actions">
              <button class="header-action-btn" title="Video Call">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
              </button>
              <button class="header-action-btn" title="More options">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
                </svg>
              </button>
            </div>
          </div>

          <!-- Messages Area -->
          <div class="messages-container" ref="messagesContainer">
            <div 
              v-for="message in messages" 
              :key="message.id"
              :class="['message', { 'message-sent': message.sent, 'message-received': !message.sent }]"
            >
              <div class="message-content">
                <p class="message-text">{{ message.text }}</p>
                <span class="message-time">{{ message.time }}</span>
              </div>
            </div>
          </div>

          <!-- Message Input -->
          <div class="message-input-container">
            <div class="input-actions">
              <button class="input-action-btn" @click="toggleEmojiPicker" title="Emoji">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </button>
              <button class="input-action-btn" @click="attachFile" title="Attach file">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
                </svg>
              </button>
            </div>
            <input 
              type="text" 
              v-model="newMessage"
              @keyup.enter="sendMessage"
              class="message-input" 
              placeholder="Type a message..."
            />
            <button 
              @click="sendMessage" 
              :disabled="!newMessage.trim()"
              class="send-button"
              :class="{ 'send-button-active': newMessage.trim() }"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Chat',
  data() {
    return {
      searchQuery: '',
      selectedChat: null,
      newMessage: '',
      chats: [
        {
          id: 1,
          name: 'Dr. Emma Watson',
          avatar: 'https://cdn.pixabay.com/photo/2018/01/15/07/51/woman-3083383__340.jpg',
          lastMessage: 'Good Morning! How can I help you today?',
          lastMessageTime: '2:30 PM',
          online: true,
          unread: 2
        },
        {
          id: 2,
          name: 'Dr. John Smith',
          avatar: 'https://cdn.pixabay.com/photo/2018/09/12/12/14/man-3672010__340.jpg',
          lastMessage: 'The test results look good',
          lastMessageTime: '1:15 PM',
          online: false,
          unread: 0
        },
        {
          id: 3,
          name: 'Dr. Sarah Johnson',
          avatar: 'https://cdn.pixabay.com/photo/2016/11/29/13/14/attractive-1869761__340.jpg',
          lastMessage: 'Please schedule a follow-up appointment',
          lastMessageTime: '11:45 AM',
          online: true,
          unread: 1
        }
      ],
      messages: [
        { id: 1, text: 'Hi, how can I help you today?', sent: false, time: '2:25 PM' },
        { id: 2, text: 'Hello! I have a question about my prescription', sent: true, time: '2:26 PM' },
        { id: 3, text: 'Of course! What would you like to know?', sent: false, time: '2:27 PM' }
      ]
    }
  },
  computed: {
    filteredChats() {
      if (!this.searchQuery) return this.chats
      const query = this.searchQuery.toLowerCase()
      return this.chats.filter(chat => 
        chat.name.toLowerCase().includes(query) ||
        chat.lastMessage.toLowerCase().includes(query)
      )
    }
  },
  methods: {
    selectChat(chat) {
      this.selectedChat = chat
      // Mark as read
      chat.unread = 0
      // Scroll to bottom of messages
      this.$nextTick(() => {
        this.scrollToBottom()
      })
    },
    sendMessage() {
      if (!this.newMessage.trim() || !this.selectedChat) return
      
      const now = new Date()
      const time = now.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
      
      this.messages.push({
        id: this.messages.length + 1,
        text: this.newMessage,
        sent: true,
        time: time
      })
      
      this.newMessage = ''
      this.selectedChat.lastMessage = this.messages[this.messages.length - 1].text
      this.selectedChat.lastMessageTime = time
      
      this.$nextTick(() => {
        this.scrollToBottom()
      })
    },
    scrollToBottom() {
      const container = this.$refs.messagesContainer
      if (container) {
        container.scrollTop = container.scrollHeight
      }
    },
    startNewChat() {
      // Implementation for starting new chat
      console.log('Start new chat')
    },
    toggleEmojiPicker() {
      // Implementation for emoji picker
      console.log('Toggle emoji picker')
    },
    attachFile() {
      // Implementation for file attachment
      console.log('Attach file')
    }
  },
  mounted() {
    // Auto-select first chat if available
    if (this.chats.length > 0 && !this.selectedChat) {
      this.selectChat(this.chats[0])
    }
  }
}
</script>

<style scoped>
.chat-container {
  @apply h-full w-full p-4 bg-gray-50 dark:bg-gray-900;
}

.chat-wrapper {
  @apply max-w-7xl mx-auto h-[calc(100vh-8rem)] bg-white dark:bg-gray-800 rounded-lg shadow-lg overflow-hidden;
  display: grid;
  grid-template-columns: 350px 1fr;
}

/* Sidebar Styles */
.chat-sidebar {
  @apply border-r border-gray-200 dark:border-gray-700 flex flex-col bg-white dark:bg-gray-800;
}

.sidebar-header {
  @apply p-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between;
}

.new-chat-btn {
  @apply p-2 rounded-lg bg-indigo-100 dark:bg-indigo-900 text-indigo-600 dark:text-indigo-300 hover:bg-indigo-200 dark:hover:bg-indigo-800 transition-colors;
}

.search-container {
  @apply p-4 border-b border-gray-200 dark:border-gray-700;
}

.search-input-wrapper {
  @apply relative;
}

.search-icon {
  @apply absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400;
}

.search-input {
  @apply w-full pl-10 pr-4 py-2 bg-gray-100 dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:text-gray-200;
}

.chat-list {
  @apply flex-1 overflow-y-auto;
}

.chat-item {
  @apply flex items-center p-4 border-b border-gray-100 dark:border-gray-700 cursor-pointer transition-colors hover:bg-gray-50 dark:hover:bg-gray-700;
}

.chat-item.active {
  @apply bg-indigo-50 dark:bg-indigo-900/20 border-l-4 border-indigo-500;
}

.chat-avatar-wrapper {
  @apply relative mr-3;
}

.chat-avatar {
  @apply w-12 h-12 rounded-full object-cover;
}

.online-indicator {
  @apply absolute bottom-0 right-0 w-3 h-3 bg-green-500 rounded-full border-2 border-white dark:border-gray-800;
}

.chat-info {
  @apply flex-1 min-w-0;
}

.chat-header-info {
  @apply flex items-center justify-between mb-1;
}

.chat-name {
  @apply font-semibold text-gray-800 dark:text-gray-200 truncate;
}

.chat-time {
  @apply text-xs text-gray-500 dark:text-gray-400;
}

.chat-preview {
  @apply flex items-center justify-between;
}

.chat-message {
  @apply text-sm text-gray-600 dark:text-gray-400 truncate flex-1;
}

.unread-badge {
  @apply ml-2 px-2 py-0.5 bg-indigo-500 text-white text-xs rounded-full font-semibold;
}

/* Main Chat Area */
.chat-main {
  @apply flex flex-col bg-white dark:bg-gray-800;
}

.no-chat-selected {
  @apply flex items-center justify-center h-full;
}

.no-chat-content {
  @apply text-center;
}

.chat-window {
  @apply flex flex-col h-full;
}

.chat-window-header {
  @apply p-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between bg-white dark:bg-gray-800;
}

.header-user-info {
  @apply flex items-center;
}

.header-avatar-wrapper {
  @apply relative mr-3;
}

.header-avatar {
  @apply w-10 h-10 rounded-full object-cover;
}

.header-online-indicator {
  @apply absolute bottom-0 right-0 w-3 h-3 bg-green-500 rounded-full border-2 border-white dark:border-gray-800;
}

.header-name {
  @apply font-semibold text-gray-800 dark:text-gray-200;
}

.header-status {
  @apply text-sm text-gray-500 dark:text-gray-400;
}

.header-actions {
  @apply flex items-center gap-2;
}

.header-action-btn {
  @apply p-2 rounded-lg text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors;
}

.messages-container {
  @apply flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50 dark:bg-gray-900/50;
}

.message {
  @apply flex;
}

.message-sent {
  @apply justify-end;
}

.message-received {
  @apply justify-start;
}

.message-content {
  @apply max-w-xs lg:max-w-md px-4 py-2 rounded-lg;
}

.message-sent .message-content {
  @apply bg-indigo-500 text-white rounded-br-none;
}

.message-received .message-content {
  @apply bg-white dark:bg-gray-700 text-gray-800 dark:text-gray-200 rounded-bl-none shadow;
}

.message-text {
  @apply mb-1;
}

.message-time {
  @apply text-xs opacity-70;
}

.message-input-container {
  @apply p-4 border-t border-gray-200 dark:border-gray-700 flex items-center gap-2 bg-white dark:bg-gray-800;
}

.input-actions {
  @apply flex items-center gap-2;
}

.input-action-btn {
  @apply p-2 rounded-lg text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors;
}

.message-input {
  @apply flex-1 px-4 py-2 bg-gray-100 dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded-full focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:text-gray-200;
}

.send-button {
  @apply p-2 rounded-full text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors disabled:opacity-50 disabled:cursor-not-allowed;
}

.send-button-active {
  @apply text-indigo-500 hover:text-indigo-600 dark:text-indigo-400;
}

@media (max-width: 1024px) {
  .chat-wrapper {
    grid-template-columns: 1fr;
  }
  
  .chat-sidebar {
    display: none;
  }
}
</style>
