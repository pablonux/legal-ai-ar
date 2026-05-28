# Work Item — Saved Conversations

**Created**: 2026-03-25
**Status**: Draft
**Feature**: Advanced AI Assistant

## User Story

As a **lawyer**, I want **to save, name, and resume my chat conversations with the assistant** so that **I can organize my research by case or topic and continue a line of inquiry across multiple sessions without losing context**.

## Context

- **Current state**: Chat is session-based. When the user navigates away or refreshes, the conversation is lost. There is no persistence or history.
- **Target state**: Conversations are automatically persisted (Azure SQL). The chat view shows a sidebar with saved conversations grouped by date. Users can rename conversations, delete them, and resume any previous conversation with full context. New conversations start from a "New chat" button.
- **Data model**: `Conversation` entity (id, userId, title, createdAt, updatedAt) → `Message` entities (id, conversationId, role, content, toolEvents, timestamp).
- **Reference**: ChatGPT conversation history, CoCounsel research sessions, Sapphire Legal AI chat history.
