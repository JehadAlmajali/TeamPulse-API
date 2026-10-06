-- Team Pulse API: DEVELOPMENT-ONLY draft seed data
-- Five original example questions appear first, followed by 25 drafted questions.
-- ALL categories and the additional 25 questions are proposals requiring supervisor approval for production.
-- Run ONLY against the local DEVELOPMENT database after applying InitialCreate and PreventBlankQuestions migrations.
-- Before running, replace NULL below with a legitimate, explicitly authorized development/test creator ID.
-- Do not invent or reuse a production employee ID. Do not run this draft seed on production.
-- This script deliberately refuses to change an existing, nonempty Questions repository.

BEGIN;

DO $seed$
DECLARE
    seed_creator_id BIGINT := 0; -- REPLACE with your authorized local development/test creator ID
BEGIN
    IF seed_creator_id IS NULL THEN
        RAISE EXCEPTION 'Set seed_creator_id to an authorized development/test creator ID before running this script.';
    END IF;

    IF EXISTS (SELECT 1 FROM "Questions") THEN
        RAISE EXCEPTION 'Questions table is not empty. Review existing data instead of running the initial seed again.';
    END IF;

    INSERT INTO "Questions"
        ("DisplayOrder", "Question", "Category", "IsActive", "CreatedBy", "CreatedDate")
    VALUES
        ( 1, 'I clearly understand today''s priorities.',                                     'Clarity',       TRUE, seed_creator_id, NOW()),
        ( 2, 'I have everything I need to perform my work effectively.',                     'Resources',     TRUE, seed_creator_id, NOW()),
        ( 3, 'Communication within my team is effective.',                                   'Communication', TRUE, seed_creator_id, NOW()),
        ( 4, 'I received the support I needed today.',                                       'Support',       TRUE, seed_creator_id, NOW()),
        ( 5, 'I feel my work is appreciated.',                                                'Recognition',   TRUE, seed_creator_id, NOW()),
        ( 6, 'I know which tasks are most important when priorities change.',                'Clarity',       TRUE, seed_creator_id, NOW()),
        ( 7, 'I can access the tools I need without unnecessary delays.',                    'Resources',     TRUE, seed_creator_id, NOW()),
        ( 8, 'Important updates are shared with me in a timely manner.',                     'Communication', TRUE, seed_creator_id, NOW()),
        ( 9, 'I know whom to contact when I need help.',                                     'Support',       TRUE, seed_creator_id, NOW()),
        (10, 'My contributions are acknowledged by my team.',                                'Recognition',   TRUE, seed_creator_id, NOW()),
        (11, 'I understand how my work contributes to my team''s goals.',                     'Clarity',       TRUE, seed_creator_id, NOW()),
        (12, 'I have enough information to complete my assigned tasks.',                     'Resources',     TRUE, seed_creator_id, NOW()),
        (13, 'I feel comfortable asking questions within my team.',                          'Communication', TRUE, seed_creator_id, NOW()),
        (14, 'I receive useful guidance when I face a new challenge.',                       'Support',       TRUE, seed_creator_id, NOW()),
        (15, 'My efforts are recognized when I do a good job.',                              'Recognition',   TRUE, seed_creator_id, NOW()),
        (16, 'I know what successful completion of my tasks looks like.',                    'Clarity',       TRUE, seed_creator_id, NOW()),
        (17, 'I have a manageable workload for the time available.',                         'Resources',     TRUE, seed_creator_id, NOW()),
        (18, 'I have opportunities to share feedback about how our team works.',             'Communication', TRUE, seed_creator_id, NOW()),
        (19, 'My teammates are willing to help when I need assistance.',                     'Support',       TRUE, seed_creator_id, NOW()),
        (20, 'I receive feedback that helps me understand the value of my work.',            'Recognition',   TRUE, seed_creator_id, NOW()),
        (21, 'I am informed when my work priorities change.',                               'Clarity',       TRUE, seed_creator_id, NOW()),
        (22, 'The information I need is easy to find.',                                       'Resources',     TRUE, seed_creator_id, NOW()),
        (23, 'Team meetings help me stay informed about our progress.',                      'Communication', TRUE, seed_creator_id, NOW()),
        (24, 'I can raise work-related concerns and expect them to be heard.',               'Support',       TRUE, seed_creator_id, NOW()),
        (25, 'Good work is acknowledged fairly within my team.',                             'Recognition',   TRUE, seed_creator_id, NOW()),
        (26, 'I understand who is responsible for the tasks I collaborate on.',              'Clarity',       TRUE, seed_creator_id, NOW()),
        (27, 'I have sufficient time to complete my tasks to a good standard.',               'Resources',     TRUE, seed_creator_id, NOW()),
        (28, 'Communication between my team and other teams is effective.',                  'Communication', TRUE, seed_creator_id, NOW()),
        (29, 'I receive the guidance I need to improve my performance.',                     'Support',       TRUE, seed_creator_id, NOW()),
        (30, 'I feel my contributions make a meaningful difference to the team.',            'Recognition',   TRUE, seed_creator_id, NOW());
END
$seed$;

COMMIT;

-- Verification: should return 30 active questions if the insert succeeds.
SELECT COUNT(*) AS question_count,
       COUNT(*) FILTER (WHERE "IsActive") AS active_count,
       MIN("DisplayOrder") AS first_order,
       MAX("DisplayOrder") AS last_order
FROM "Questions";

SELECT "DisplayOrder", "Question", "Category", "IsActive"
FROM "Questions"
ORDER BY "DisplayOrder";
